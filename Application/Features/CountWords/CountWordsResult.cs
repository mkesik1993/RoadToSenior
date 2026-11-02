namespace WordAnalytics.Application.Features.CountWords;

public record CountWordsResult(IReadOnlyList<KeyValuePair<string, int>> Counts);
