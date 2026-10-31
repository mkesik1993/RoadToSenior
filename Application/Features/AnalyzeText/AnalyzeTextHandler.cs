using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application.Features.AnalyzeText
{
    public class AnalyzeTextHandler(IWordCounter counter, IWordRanker ranker, IOptions<RankingOptions> options, ILogger<AnalyzeTextHandler> logger)
    {
        private readonly RankingOptions _options = options.Value;

        public AnalyzeTextResult Handle(AnalyzeTextInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var counts = counter.CountWords(input.Text);

            logger.LogInformation("Found {Words} words.", counts.Count);

            return new AnalyzeTextResult(ranker.GetTop(counts, input.TopCount ?? _options.TopCount));
        }
    }
}
