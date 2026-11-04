using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using WordAnalytics.Api.Features.GetTopWords;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests.Integration;

public class GetTopWordsEndpointTests(WordAnalyticsApiFactory factory) : IClassFixture<WordAnalyticsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetTopWords_WithValidCounts_ReturnsTopInDescendingOrder()
    {
        const string json = """
            {"counts":[{"word":"a","count":1},{"word":"b","count":3},{"word":"c","count":3},{"word":"d","count":2},{"word":"e","count":5}],"topCount":4}
            """;
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/top-words", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await Verifier.VerifyJson(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetTopWords_WithoutTopCount_UsesConfiguredDefault()
    {
        var response = await _client.PostAsJsonAsync("/api/top-words", new
        {
            Counts = new[] { new { Word = "a", Count = 1 }, new { Word = "b", Count = 2 }, new { Word = "c", Count = 3 }, new { Word = "d", Count = 4 } }
        });
        var result = await response.Content.ReadFromJsonAsync<GetTopWordsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Top.Count);
    }

    [Fact]
    public async Task GetTopWords_WithEmptyCounts_ReturnsEmptyTop()
    {
        var response = await _client.PostAsJsonAsync("/api/top-words", new { Counts = Array.Empty<object>() });
        var result = await response.Content.ReadFromJsonAsync<GetTopWordsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Top);
    }

    [Fact]
    public async Task GetTopWords_WithTopCountAboveAvailable_ReturnsAll()
    {
        var response = await _client.PostAsJsonAsync("/api/top-words", new
        {
            Counts = new[] { new { Word = "a", Count = 1 }, new { Word = "b", Count = 2 } },
            TopCount = 50
        });
        var result = await response.Content.ReadFromJsonAsync<GetTopWordsResponse>();

        Assert.NotNull(result);
        Assert.Equal(2, result.Top.Count);
    }

    [Theory]
    [InlineData("{}", "Counts")]
    [InlineData("{\"counts\":null}", "Counts")]
    [InlineData("{\"counts\":[],\"topCount\":0}", "TopCount")]
    [InlineData("{\"counts\":[],\"topCount\":101}", "TopCount")]
    [InlineData("{\"counts\":[{\"word\":\"a\",\"count\":0}]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"word\":\"a\",\"count\":-1}]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"count\":1}]}", "Counts[0]")]
    [InlineData("{\"counts\":[null]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"word\":\"Hello\",\"count\":1}]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"word\":\"two words\",\"count\":1}]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"word\":\"a!\",\"count\":1}]}", "Counts[0]")]
    [InlineData("{\"counts\":[{\"word\":\"a\",\"count\":1},{\"word\":\"b\",\"count\":1},{\"word\":\"a\",\"count\":2}]}", "Counts[2]")]
    public async Task GetTopWords_WithInvalidInput_Returns400(string json, string expectedMember)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/top-words", content);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey(expectedMember));
    }

    [Fact]
    public async Task GetTopWords_WithTooManyEntries_Returns400()
    {
        var counts = Enumerable.Range(0, GetTopWordsRequest.MaxEntries + 1)
            .Select(i => new { Word = $"w{i}", Count = 1 })
            .ToArray();

        var response = await _client.PostAsJsonAsync("/api/top-words", new { Counts = counts });
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("Counts"));
    }

    [Fact]
    public async Task GetTopWords_WithTooLongWord_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/top-words", new
        {
            Counts = new[] { new { Word = new string('a', GetTopWordsRequest.MaxWordLength + 1), Count = 1 } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTopWords_WhenRankerFails_ReturnsGeneric500()
    {
        var ranker = new Mock<IWordRanker>();
        ranker.Setup(service => service.GetTop(It.IsAny<Dictionary<string, int>>(), It.IsAny<int>()))
            .Throws(new InvalidOperationException("Simulated ranker failure."));

        await using var failingFactory = new WordAnalyticsApiFactory()
            .WithServices(services =>
            {
                services.RemoveAll<IWordRanker>();
                services.AddSingleton<IWordRanker>(ranker.Object);
            });
        using var client = failingFactory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/top-words",
            new { Counts = new[] { new { Word = "a", Count = 1 } } });
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("An unexpected error occurred.", problem.Title);
        Assert.DoesNotContain("Simulated ranker failure.", await response.Content.ReadAsStringAsync());
    }
}
