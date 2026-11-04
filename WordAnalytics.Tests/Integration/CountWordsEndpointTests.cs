using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using WordAnalytics.Api.Features.CountWords;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests.Integration;

public class CountWordsEndpointTests(WordAnalyticsApiFactory factory) : IClassFixture<WordAnalyticsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CountWords_WithValidText_ReturnsAllCounts()
    {
        // Arrange
        var request = new CountWordsRequest { Text = "World, hello HELLO! Code code code. extra another." };

        // Act
        var response = await _client.PostAsJsonAsync("/api/word-counts", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await Verifier.VerifyJson(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CountWords_WithPunctuationOnly_ReturnsEmptyCounts()
    {
        // Arrange
        var request = new CountWordsRequest { Text = "!!! ..." };

        // Act
        var response = await _client.PostAsJsonAsync("/api/word-counts", request);
        var result = await response.Content.ReadFromJsonAsync<CountWordsResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Counts);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"text\":null}")]
    [InlineData("{\"text\":\"\"}")]
    [InlineData("{\"text\":\"   \"}")]
    public async Task CountWords_WithMissingOrBlankText_Returns400(string json)
    {
        // Arrange
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/word-counts", content);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("Text"));
    }

    [Fact]
    public async Task CountWords_WithTooLongText_Returns400()
    {
        // Arrange
        var request = new CountWordsRequest { Text = new string('a', 1_000_001) };

        // Act
        var response = await _client.PostAsJsonAsync("/api/word-counts", request);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("Text"));
    }

    [Fact]
    public async Task CountWords_WhenCounterFails_ReturnsGeneric500()
    {
        // Arrange
        var counter = new Mock<IWordCounter>();
        counter.Setup(service => service.CountWords(It.IsAny<string?>()))
            .Throws(new InvalidOperationException("Simulated counter failure."));

        await using var failingFactory = new WordAnalyticsApiFactory()
            .WithServices(services =>
            {
                services.RemoveAll<IWordCounter>();
                services.AddSingleton<IWordCounter>(counter.Object);
            });
        using var client = failingFactory.CreateClient();
        var request = new CountWordsRequest { Text = "hello" };

        // Act
        var response = await client.PostAsJsonAsync("/api/word-counts", request);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("An unexpected error occurred.", problem.Title);
        Assert.DoesNotContain("Simulated counter failure.", await response.Content.ReadAsStringAsync());
    }
}
