namespace WordAnalytics.Api.Contracts.Responses
{
    public class AnalyzeResponse
    {
        public required IReadOnlyList<WordFrequencyDto> Top { get; init; }
    }
}
