using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Application.Features.CreateAnalysis;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class CreateAnalysisHandlerTests
{
    private static readonly DateTimeOffset CreatedAtUtc = new(2026, 11, 12, 8, 0, 0, TimeSpan.Zero);

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
        Assert.Equal(CreatedAtUtc, result.CreatedAtUtc);
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
    public async Task HandleAsync_WithSubMicrosecondTime_PreservesTimestampPrecision()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var timeWithSubMicrosecondTicks = CreatedAtUtc.AddTicks(7);
        var handler = CreateHandler(store, timeWithSubMicrosecondTicks);

        // Act
        var result = await handler.HandleAsync(new CreateAnalysisInput("hello"));

        // Assert
        Assert.Equal(timeWithSubMicrosecondTicks, result.CreatedAtUtc);
        Assert.Equal(timeWithSubMicrosecondTicks, store.LastAdded?.CreatedAtUtc);
    }

    private static CreateAnalysisHandler CreateHandler(
        FakeAnalysisStore store,
        int defaultTopCount = 3)
    {
        return CreateHandler(store, CreatedAtUtc, defaultTopCount);
    }

    private static CreateAnalysisHandler CreateHandler(
        FakeAnalysisStore store,
        DateTimeOffset utcNow,
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
            Options.Create(new RankingOptions { TopCount = defaultTopCount }),
            new FixedTimeProvider(utcNow));
    }

    private class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return utcNow;
        }
    }
}
