using WordAnalytics.Counting.Interfaces;

namespace WordAnalytics.Counting.Services
{
    public class WordCounter : IWordCounter
    {
        public Dictionary<string, int> CountWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new Dictionary<string, int>();

            return string.Concat(input.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : ' '))
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => w)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
