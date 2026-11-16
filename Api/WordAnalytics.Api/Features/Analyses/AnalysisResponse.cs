namespace WordAnalytics.Api.Features.Analyses;

public class AnalysisResponse
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public required int TopCount { get; init; }
    public required IReadOnlyList<AnalysisWordDto> TopWords { get; init; }
}
