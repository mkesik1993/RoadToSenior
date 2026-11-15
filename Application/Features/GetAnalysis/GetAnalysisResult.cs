namespace WordAnalytics.Application.Features.GetAnalysis;

public record GetAnalysisResult(
    Guid Id,
    string Text,
    DateTimeOffset CreatedAtUtc,
    int TopCount,
    IReadOnlyList<KeyValuePair<string, int>> TopWords);
