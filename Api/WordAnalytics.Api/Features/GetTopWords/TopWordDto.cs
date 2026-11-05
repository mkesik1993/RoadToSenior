namespace WordAnalytics.Api.Features.GetTopWords;

public class TopWordDto
{
    public required string Word { get; init; }
    public required int Count { get; init; }
}
