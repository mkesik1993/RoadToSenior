namespace WordAnalytics.Application.Features.CreateAnalysis;

public record CreateAnalysisResult(
    Guid Id,
    string Text,
    DateTimeOffset CreatedAtUtc,
    int TopCount,
    IReadOnlyList<KeyValuePair<string, int>> TopWords);
