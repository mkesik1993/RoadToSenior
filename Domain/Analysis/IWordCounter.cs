namespace WordAnalytics.Domain.Analysis
{
    public interface IWordCounter
    {
        IReadOnlyDictionary<string, int> CountWords(string? input);
    }
}
