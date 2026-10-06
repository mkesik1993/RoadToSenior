using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Ranking.Settings
{
    public sealed class RankingOptions
    {
        public const string SectionName = "Ranking";

        [Range(1, 1000, ErrorMessage = "TopCount must be between 1 and 1000.")]
        public int TopCount { get; init; } = 3;
    }
}
