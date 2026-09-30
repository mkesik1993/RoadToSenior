using Microsoft.Extensions.Logging;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Ranking.Interfaces;

namespace WordAnalytics.Cli
{
    public sealed class TextAnalysisService(
        IWordCounter counter,
        IWordRanker ranker,
        ILogger<TextAnalysisService> logger)
    {
        public List<KeyValuePair<string, int>> Analyze(string input)
        {
            var counts = counter.CountWords(input);

            logger.LogInformation("Found {Words} words.", counts.Count);

            return ranker.GetTop(counts);
        }
    }
}
