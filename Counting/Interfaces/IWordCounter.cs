namespace WordAnalytics.Counting.Interfaces
{
    public interface IWordCounter
    {
        public Dictionary<string, int> CountWords(string? input);
    }
}
