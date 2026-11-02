namespace WordAnalytics.Api.Features.CountWords;

public class WordCountDto
{
    public required string Word { get; init; }
    public required int Count { get; init; }
}
