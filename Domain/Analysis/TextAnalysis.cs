namespace WordAnalytics.Domain.Analysis;

public class TextAnalysis
{
    public const int MaxTextLength = 1_000_000;
    private readonly IReadOnlyList<KeyValuePair<string, int>> _topWords;

    private TextAnalysis(
        Guid id,
        string text,
        DateTimeOffset createdAtUtc,
        int topCount,
        IReadOnlyList<KeyValuePair<string, int>> topWords)
    {
        Id = id;
        Text = text;
        CreatedAtUtc = createdAtUtc;
        TopCount = topCount;
        _topWords = topWords;
    }

    public Guid Id { get; }
    public string Text { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public int TopCount { get; }
    public IReadOnlyList<KeyValuePair<string, int>> TopWords => _topWords;

    public static TextAnalysis Create(
        Guid id,
        string text,
        DateTimeOffset createdAtUtc,
        int topCount,
        IEnumerable<KeyValuePair<string, int>> topWords)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Analysis ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Analysis text is required.");
        }

        if (text.Length > MaxTextLength)
        {
            throw new ArgumentException($"Analysis text cannot exceed {MaxTextLength} characters.");
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Analysis creation time must be UTC.");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(topCount, 1);

        ArgumentNullException.ThrowIfNull(topWords);

        var words = topWords.ToArray();

        if (words.Length > topCount)
        {
            throw new ArgumentException("Top words cannot exceed TopCount.");
        }

        var seenWords = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (word, count) in words)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentException("Top words cannot be empty.");
            }

            if (count < 1)
            {
                throw new ArgumentException("Word counts must be greater than zero.");
            }

            if (!seenWords.Add(word))
            {
                throw new ArgumentException("Top words must be unique.");
            }
        }

        return new TextAnalysis(id, text, createdAtUtc, topCount, Array.AsReadOnly(words));
    }
}
