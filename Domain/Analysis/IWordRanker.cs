namespace WordAnalytics.Domain.Analysis
{
    public interface IWordRanker
    {
        public List<KeyValuePair<string, int>> GetTop(Dictionary<string, int> counts, int take);
    }
}
