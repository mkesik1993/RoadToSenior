using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;
using WordAnalytics.Api.Features.Analyses;
using WordAnalytics.Api.Features.CreateAnalysis;
using WordAnalytics.Application.AnalysisHistory;
using WordAnalytics.Tests.Integration.TestSupport;
using WordAnalytics.Tests.TestSupport;

namespace WordAnalytics.Tests.Integration.Api;

public class SavedAnalysisEndpointsTests
{
    [Fact]
    public async Task CreateAnalysis_WithValidRequest_Returns201AndCanBeRetrieved()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        await using var factory = CreateFactory(store);
        using var client = factory.CreateClient();
        var request = new CreateAnalysisRequest { Text = "blue blue red", TopCount = 2 };
        var expectedTopWords = new[]
        {
            new KeyValuePair<string, int>("blue", 2),
            new KeyValuePair<string, int>("red", 1)
        };

        // Act
        var createResponse = await client.PostAsJsonAsync("/api/analyses", request);
        var createdAnalysis = await createResponse.Content.ReadFromJsonAsync<AnalysisResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        Assert.NotNull(createdAnalysis);
        Assert.Equal(request.Text, createdAnalysis.Text);
        Assert.Equal(2, createdAnalysis.TopCount);
        Assert.Equal(
            expectedTopWords,
            createdAnalysis.TopWords
                .Select(word => new KeyValuePair<string, int>(word.Word, word.Count)));

        // Act
        var getResponse = await client.GetAsync(createResponse.Headers.Location);
        var retrievedAnalysis = await getResponse.Content.ReadFromJsonAsync<AnalysisResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(retrievedAnalysis);
        Assert.Equal(createdAnalysis.Id, retrievedAnalysis.Id);
        Assert.Equal(createdAnalysis.Text, retrievedAnalysis.Text);
        Assert.Equal(
            expectedTopWords,
            retrievedAnalysis.TopWords
                .Select(word => new KeyValuePair<string, int>(word.Word, word.Count)));
    }

    [Theory]
    [InlineData("", 3, "Text")]
    [InlineData("hello", 0, "TopCount")]
    [InlineData("hello", 101, "TopCount")]
    public async Task CreateAnalysis_WithInvalidRequest_Returns400(
        string text,
        int topCount,
        string expectedMember)
    {
        // Arrange
        var store = new FakeAnalysisStore();
        await using var factory = CreateFactory(store);
        using var client = factory.CreateClient();
        var request = new CreateAnalysisRequest { Text = text, TopCount = topCount };

        // Act
        var response = await client.PostAsJsonAsync("/api/analyses", request);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey(expectedMember));
    }

    [Fact]
    public async Task GetAnalysis_WithMissingId_Returns404()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        await using var factory = CreateFactory(store);
        using var client = factory.CreateClient();
        var id = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/analyses/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static WordAnalyticsApiFactory CreateFactory(FakeAnalysisStore store)
    {
        return new WordAnalyticsApiFactory()
            .WithServices(services =>
            {
                services.RemoveAll<IAnalysisStore>();
                services.AddSingleton<IAnalysisStore>(store);
            });
    }
}
