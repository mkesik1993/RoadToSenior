namespace WordAnalytics.Application.Features.CreateAnalysis;

public record CreateAnalysisInput(string Text, int? TopCount = null);
