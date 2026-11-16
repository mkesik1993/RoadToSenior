using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application.AnalysisHistory;

public interface IAnalysisStore
{
    Task AddAsync(TextAnalysis analysis, CancellationToken cancellationToken);

    Task<TextAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
