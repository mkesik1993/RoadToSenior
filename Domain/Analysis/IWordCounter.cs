namespace WordAnalytics.Domain.Analysis
{
    public interface IWordCounter
    {
        public Dictionary<string, int> CountWords(string? input);
    }
}
