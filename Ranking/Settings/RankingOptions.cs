namespace WordAnalytics.Ranking.Settings
{
    public sealed class RankingOptions
    {
        public const string SectionName = "Ranking";

        public int TopCount { get; set; } = 3;
    }
}
