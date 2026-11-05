using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Api.Features.CountWords;

public class CountWordsRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Text is required.")]
    [StringLength(1_000_000, ErrorMessage = "Text cannot exceed 1 000 000 characters.")]
    public string Text { get; init; } = string.Empty;
}
