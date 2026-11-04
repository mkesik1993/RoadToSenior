namespace WordAnalytics.Application.Features.GetTopWords;

public record GetTopWordsInput(IReadOnlyList<KeyValuePair<string, int>> Counts, int? TopCount);
