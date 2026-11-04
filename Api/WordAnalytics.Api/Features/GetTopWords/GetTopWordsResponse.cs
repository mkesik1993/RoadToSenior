namespace WordAnalytics.Api.Features.GetTopWords;

public class GetTopWordsResponse
{
    public required IReadOnlyList<TopWordDto> Top { get; init; }
}
