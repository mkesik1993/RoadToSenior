using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Api.Infrastructure.RateLimiting
{
    public sealed class RateLimitingSettings
    {
        public const string SectionName = "RateLimiting";
        public const string AnalyzePolicy = "analyze";

        [Range(1, 10_000)]
        public int PermitLimit { get; init; } = 10;

        [Range(1, 3_600)]
        public int WindowSeconds { get; init; } = 60;

        [Range(0, 1_000)]
        public int QueueLimit { get; init; } = 0;
    }
}
