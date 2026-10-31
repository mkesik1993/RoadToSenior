using Microsoft.Extensions.Logging;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Ranking.Interfaces;

namespace WordAnalytics.Application.Features.AnalyzeText
{
    public class AnalyzeTextHandler(IWordCounter counter, IWordRanker ranker, ILogger<AnalyzeTextHandler> logger)
    {
        public AnalyzeTextResult Handle(AnalyzeTextInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var counts = counter.CountWords(input.Text);

            logger.LogInformation("Found {Words} words.", counts.Count);

            return new AnalyzeTextResult(ranker.GetTop(counts, input.TopCount));
        }
    }
}
