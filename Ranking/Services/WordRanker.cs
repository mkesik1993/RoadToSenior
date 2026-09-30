using Microsoft.Extensions.Options;
using WordAnalytics.Ranking.Interfaces;
using WordAnalytics.Ranking.Settings;

namespace WordAnalytics.Ranking.Services
{
    public class WordRanker(IOptions<RankingOptions> options) : IWordRanker
    {
        private readonly RankingOptions _options = options.Value;

        public List<KeyValuePair<string, int>> GetTop(Dictionary<string, int> counts)
        {
            return counts.OrderByDescending(kv => kv.Value).Take(_options.TopCount).ToList();
        }
    }
}
