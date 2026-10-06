namespace WordAnalytics.Ranking.Interfaces
{
    public interface IWordRanker
    {
        public List<KeyValuePair<string, int>> GetTop(Dictionary<string, int> counts, int? take = null);
    }
}
