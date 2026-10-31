namespace WordAnalytics.Application.Features.AnalyzeText;

public record AnalyzeTextResult(IReadOnlyList<KeyValuePair<string, int>> Top);
