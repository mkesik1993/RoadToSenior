using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Application.Features.CountWords;
using WordAnalytics.Application.Features.GetTopWords;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class GetTopWordsHandlerTests
{
    private static readonly KeyValuePair<string, int>[] Counts =
    [
        new("a", 1), new("b", 3), new("c", 3), new("d", 2), new("e", 5)
    ];

    private static GetTopWordsHandler CreateHandler(int defaultTopCount = 3)
    {
        return new GetTopWordsHandler(
            new WordRanker(),
            Options.Create(new RankingOptions { TopCount = defaultTopCount }));
    }

    [Fact]
    public void Handle_WithExplicitTopCount_ReturnsOrderedTopWithStableTies()
    {
        // Arrange
        var handler = CreateHandler();
        var input = new GetTopWordsInput(Counts, 3);

        // Act
        var result = handler.Handle(input);

        // Assert
        Assert.Equal([new("e", 5), new("b", 3), new("c", 3)], result.Top);
    }

    [Fact]
    public void Handle_WithoutTopCount_UsesConfiguredDefault()
    {
        // Arrange
        var handler = CreateHandler(defaultTopCount: 2);
        var input = new GetTopWordsInput(Counts, null);

        // Act
        var result = handler.Handle(input);

        // Assert
        Assert.Equal(2, result.Top.Count);
    }

    [Fact]
    public void Handle_WithTopCountAboveAvailable_ReturnsAll()
    {
        // Arrange
        var handler = CreateHandler();
        var input = new GetTopWordsInput(Counts, 100);

        // Act
        var result = handler.Handle(input);

        // Assert
        Assert.Equal(Counts.Length, result.Top.Count);
    }

    [Fact]
    public void Handle_WithNoCounts_ReturnsEmpty()
    {
        // Arrange
        var handler = CreateHandler();
        var input = new GetTopWordsInput([], null);

        // Act
        var result = handler.Handle(input);

        // Assert
        Assert.Empty(result.Top);
    }

    [Fact]
    public void Handle_AfterCountWords_MatchesAnalyzeText()
    {
        // Arrange
        const string text = "World, hello HELLO! Code code code. extra another.";
        var counter = new WordCounter();
        var counts = new CountWordsHandler(counter).Handle(new CountWordsInput(text)).Counts;
        var analyzeHandler = new AnalyzeTextHandler(
            counter, new WordRanker(), Options.Create(new RankingOptions()), NullLogger<AnalyzeTextHandler>.Instance);

        // Act
        var result = CreateHandler().Handle(new GetTopWordsInput(counts, 3));
        var expected = analyzeHandler.Handle(new AnalyzeTextInput(text, 3));

        // Assert
        Assert.Equal(expected.Top, result.Top);
    }
}
