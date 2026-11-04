namespace WordAnalytics.Application.Features.GetTopWords;

public record GetTopWordsResult(IReadOnlyList<KeyValuePair<string, int>> Top);
