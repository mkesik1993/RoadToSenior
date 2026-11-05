using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Api.Features.GetTopWords;

public class GetTopWordsRequest : IValidatableObject
{
    public const int MaxEntries = 10_000;
    public const int MaxWordLength = 100;

    [Required(ErrorMessage = "Counts are required.")]
    [MaxLength(MaxEntries, ErrorMessage = "Counts cannot contain more than 10 000 entries.")]
    public List<WordCountItem>? Counts { get; init; }

    [Range(1, 100, ErrorMessage = "TopCount must be between 1 and 100.")]
    public int? TopCount { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < Counts!.Count; i++)
        {
            var item = Counts[i];
            var member = $"{nameof(Counts)}[{i}]";

            if (item is null || string.IsNullOrEmpty(item.Word))
            {
                yield return new ValidationResult("Word is required.", [member]);
                continue;
            }

            if (item.Word.Length > MaxWordLength)
                yield return new ValidationResult($"Word cannot exceed {MaxWordLength} characters.", [member]);
            else if (!IsNormalized(item.Word))
                yield return new ValidationResult("Word must contain only lowercase letters and digits.", [member]);
            else if (!seen.Add(item.Word))
                yield return new ValidationResult("Word must be unique.", [member]);

            if (item.Count < 1)
                yield return new ValidationResult("Count must be greater than zero.", [member]);
        }
    }

    private static bool IsNormalized(string word)
    {
        return word.All(c => char.IsLetterOrDigit(c) && char.ToLowerInvariant(c) == c);
    }
}
