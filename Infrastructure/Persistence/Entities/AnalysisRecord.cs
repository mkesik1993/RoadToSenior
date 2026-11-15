namespace WordAnalytics.Infrastructure.Persistence.Entities;

public class AnalysisRecord
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public int TopCount { get; set; }
    public List<AnalysisWordRecord> TopWords { get; set; } = [];
}
