namespace WordAnalytics.Infrastructure.Persistence.Entities;

public class AnalysisWordRecord
{
    public Guid AnalysisId { get; set; }
    public int Position { get; set; }
    public string Word { get; set; } = string.Empty;
    public int Count { get; set; }
    public AnalysisRecord Analysis { get; set; } = null!;
}
