using Microsoft.Extensions.Diagnostics.HealthChecks;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Ranking.Interfaces;

namespace WordAnalytics.Api.Infrastructure.HealthChecks
{
    public class WordAnalyticsHealthCheck(IWordCounter counter, IWordRanker ranker) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var counts = counter.CountWords("health check health");
            var top = ranker.GetTop(counts, 1);

            return Task.FromResult(top.Any()
                ? HealthCheckResult.Healthy("Word counting and ranking pipeline works.")
                : HealthCheckResult.Degraded("Ranking returned no results for sample text."));
        }
    }
}
