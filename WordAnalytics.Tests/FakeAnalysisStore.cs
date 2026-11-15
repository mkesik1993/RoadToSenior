using WordAnalytics.Application.AnalysisHistory;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

internal class FakeAnalysisStore : IAnalysisStore
{
    private readonly Dictionary<Guid, TextAnalysis> _analyses = [];

    public CancellationToken? AddCancellationToken { get; private set; }
    public CancellationToken? GetCancellationToken { get; private set; }
    public TextAnalysis? LastAdded { get; private set; }

    public Task AddAsync(TextAnalysis analysis, CancellationToken cancellationToken)
    {
        AddCancellationToken = cancellationToken;
        LastAdded = analysis;
        _analyses.Add(analysis.Id, analysis);

        return Task.CompletedTask;
    }

    public Task<TextAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        GetCancellationToken = cancellationToken;
        _analyses.TryGetValue(id, out var analysis);

        return Task.FromResult(analysis);
    }
}
