namespace WordAnalytics.Api.Features.CountWords;

public class CountWordsResponse
{
    public required IReadOnlyList<WordCountDto> Counts { get; init; }
}
