using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Application.Features.CreateAnalysis;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class CreateAnalysisHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithExplicitTopCount_SavesAnalysisWithInputAndRankedWords()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = CreateHandler(store, defaultTopCount: 1);
        var input = new CreateAnalysisInput("World, hello HELLO!", TopCount: 2);

        // Act
        var result = await handler.HandleAsync(input);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(input.Text, result.Text);
        Assert.Equal(2, result.TopCount);
        Assert.Equal(
            [new KeyValuePair<string, int>("hello", 2), new("world", 1)],
            result.TopWords);
        Assert.Equal(result.Id, store.LastAdded?.Id);
        Assert.Equal(result.Text, store.LastAdded?.Text);
        Assert.Equal(result.TopWords, store.LastAdded?.TopWords);
    }

    [Fact]
    public async Task HandleAsync_WithoutTopCount_UsesConfiguredDefaultForAnalysisAndStorage()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = CreateHandler(store, defaultTopCount: 2);
        var input = new CreateAnalysisInput("a a b b c c");

        // Act
        var result = await handler.HandleAsync(input);

        // Assert
        Assert.Equal(2, result.TopCount);
        Assert.Equal(2, result.TopWords.Count);
        Assert.Equal(2, store.LastAdded?.TopCount);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_PassesTokenToStore()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = CreateHandler(store);
        using var cancellationSource = new CancellationTokenSource();

        // Act
        await handler.HandleAsync(new CreateAnalysisInput("hello"), cancellationSource.Token);

        // Assert
        Assert.Equal(cancellationSource.Token, store.AddCancellationToken);
    }

    [Fact]
    public async Task HandleAsync_UsesCurrentUtcTimeAndStoresIt()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = CreateHandler(store);
        var timeBeforeHandling = DateTimeOffset.UtcNow;

        // Act
        var result = await handler.HandleAsync(new CreateAnalysisInput("hello"));
        var timeAfterHandling = DateTimeOffset.UtcNow;

        // Assert
        Assert.InRange(result.CreatedAtUtc, timeBeforeHandling, timeAfterHandling);
        Assert.Equal(TimeSpan.Zero, result.CreatedAtUtc.Offset);
        Assert.Equal(result.CreatedAtUtc, store.LastAdded?.CreatedAtUtc);
    }

    private static CreateAnalysisHandler CreateHandler(
        FakeAnalysisStore store,
        int defaultTopCount = 3)
    {
        var analyzeTextHandler = new AnalyzeTextHandler(
            new WordCounter(),
            new WordRanker(),
            Options.Create(new RankingOptions { TopCount = defaultTopCount }),
            NullLogger<AnalyzeTextHandler>.Instance);

        return new CreateAnalysisHandler(
            analyzeTextHandler,
            store,
            Options.Create(new RankingOptions { TopCount = defaultTopCount }));
    }
}
