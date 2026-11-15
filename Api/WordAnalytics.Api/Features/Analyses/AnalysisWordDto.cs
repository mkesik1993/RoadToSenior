namespace WordAnalytics.Api.Features.Analyses;

public class AnalysisWordDto
{
    public required string Word { get; init; }
    public required int Count { get; init; }
}
