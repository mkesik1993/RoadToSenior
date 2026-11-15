namespace WordAnalytics.Domain.Analysis
{
    public interface IWordRanker
    {
        IReadOnlyList<KeyValuePair<string, int>> GetTop(IReadOnlyDictionary<string, int> counts, int take);
    }
}
