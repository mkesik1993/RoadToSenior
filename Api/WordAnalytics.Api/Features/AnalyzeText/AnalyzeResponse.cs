namespace WordAnalytics.Api.Features.AnalyzeText
{
    public class AnalyzeResponse
    {
        public required IReadOnlyList<WordFrequencyDto> Top { get; init; }
    }
}
