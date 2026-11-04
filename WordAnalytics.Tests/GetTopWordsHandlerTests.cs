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
        => new(new WordRanker(), Options.Create(new RankingOptions { TopCount = defaultTopCount }));

    [Fact]
    public void Handle_WithExplicitTopCount_ReturnsOrderedTopWithStableTies()
    {
        var result = CreateHandler().Handle(new GetTopWordsInput(Counts, 3));

        Assert.Equal([new("e", 5), new("b", 3), new("c", 3)], result.Top);
    }

    [Fact]
    public void Handle_WithoutTopCount_UsesConfiguredDefault()
    {
        var result = CreateHandler(defaultTopCount: 2).Handle(new GetTopWordsInput(Counts, null));

        Assert.Equal(2, result.Top.Count);
    }

    [Fact]
    public void Handle_WithTopCountAboveAvailable_ReturnsAll()
    {
        var result = CreateHandler().Handle(new GetTopWordsInput(Counts, 100));

        Assert.Equal(Counts.Length, result.Top.Count);
    }

    [Fact]
    public void Handle_WithNoCounts_ReturnsEmpty()
    {
        var result = CreateHandler().Handle(new GetTopWordsInput([], null));

        Assert.Empty(result.Top);
    }

    [Fact]
    public void Handle_AfterCountWords_MatchesAnalyzeText()
    {
        const string text = "World, hello HELLO! Code code code. extra another.";
        var counter = new WordCounter();
        var counts = new CountWordsHandler(counter).Handle(new CountWordsInput(text)).Counts;
        var analyzeHandler = new AnalyzeTextHandler(
            counter, new WordRanker(), Options.Create(new RankingOptions()), NullLogger<AnalyzeTextHandler>.Instance);

        var result = CreateHandler().Handle(new GetTopWordsInput(counts, 3));

        Assert.Equal(analyzeHandler.Handle(new AnalyzeTextInput(text, 3)).Top, result.Top);
    }
}
