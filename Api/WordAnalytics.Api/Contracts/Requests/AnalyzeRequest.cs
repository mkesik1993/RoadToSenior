using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Api.Contracts.Requests
{
    public class AnalyzeRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Text is required.")]
        [StringLength(1_000_000, ErrorMessage = "Text cannot exceed 1 000 000 characters.")]
        public string Text { get; init; } = string.Empty;

        [Range(1, 100, ErrorMessage = "TopCount must be between 1 and 100.")]
        public int? TopCount { get; init; }
    }
}
