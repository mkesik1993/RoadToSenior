using WordAnalytics.Application.AnalysisHistory;

namespace WordAnalytics.Application.Features.GetAnalysis;

public class GetAnalysisHandler(IAnalysisStore analysisStore)
{
    public async Task<GetAnalysisResult?> HandleAsync(GetAnalysisInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var analysis = await analysisStore.GetByIdAsync(input.Id, cancellationToken);

        if (analysis is null)
        {
            return null;
        }

        return new GetAnalysisResult(
            analysis.Id,
            analysis.Text,
            analysis.CreatedAtUtc,
            analysis.TopCount,
            analysis.TopWords);
    }
}
