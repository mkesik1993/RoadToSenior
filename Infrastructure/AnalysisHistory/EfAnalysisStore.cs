using Microsoft.EntityFrameworkCore;
using WordAnalytics.Application.AnalysisHistory;
using WordAnalytics.Domain.Analysis;
using WordAnalytics.Infrastructure.Persistence;
using WordAnalytics.Infrastructure.Persistence.Entities;

namespace WordAnalytics.Infrastructure.AnalysisHistory;

public sealed class EfAnalysisStore(AnalysisDbContext dbContext) : IAnalysisStore
{
    public async Task AddAsync(TextAnalysis analysis, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(analysis);

        var record = new AnalysisRecord
        {
            Id = analysis.Id,
            Text = analysis.Text,
            CreatedAtUtc = analysis.CreatedAtUtc,
            TopCount = analysis.TopCount,
            TopWords = analysis.TopWords
                .Select((word, index) => new AnalysisWordRecord
                {
                    AnalysisId = analysis.Id,
                    Position = index + 1,
                    Word = word.Key,
                    Count = word.Value
                })
                .ToList()
        };

        dbContext.Analyses.Add(record);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TextAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await dbContext.Analyses
            .AsNoTracking()
            .Include(analysis => analysis.TopWords)
            .SingleOrDefaultAsync(analysis => analysis.Id == id, cancellationToken);

        if (record is null)
        {
            return null;
        }

        var topWords = record.TopWords
            .OrderBy(word => word.Position)
            .Select(word => new KeyValuePair<string, int>(word.Word, word.Count));

        return TextAnalysis.Create(
            record.Id,
            record.Text,
            record.CreatedAtUtc,
            record.TopCount,
            topWords);
    }
}
