using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.AnalysisHistory;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application.Features.CreateAnalysis;

public class CreateAnalysisHandler(
    AnalyzeTextHandler analyzeTextHandler,
    IAnalysisStore analysisStore,
    IOptions<RankingOptions> options)
{
    private readonly RankingOptions _options = options.Value;

    public async Task<CreateAnalysisResult> HandleAsync(CreateAnalysisInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var topCount = input.TopCount ?? _options.TopCount;
        var analysisResult = analyzeTextHandler.Handle(new AnalyzeTextInput(input.Text, topCount));

        var analysis = TextAnalysis.Create(
            Guid.NewGuid(),
            input.Text,
            DateTimeOffset.UtcNow,
            topCount,
            analysisResult.Top);

        await analysisStore.AddAsync(analysis, cancellationToken);

        return new CreateAnalysisResult(
            analysis.Id,
            analysis.Text,
            analysis.CreatedAtUtc,
            analysis.TopCount,
            analysis.TopWords);
    }
}
