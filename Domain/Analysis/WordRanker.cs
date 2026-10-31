namespace WordAnalytics.Domain.Analysis
{
    public class WordRanker : IWordRanker
    {
        public List<KeyValuePair<string, int>> GetTop(Dictionary<string, int> counts, int take)
        {
            ArgumentNullException.ThrowIfNull(counts);

            return counts.OrderByDescending(kv => kv.Value).Take(take).ToList();
        }
    }
}
