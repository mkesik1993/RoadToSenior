namespace WordAnalytics.Api.Contracts.Responses
{
    public class WordFrequencyDto
    {
        public required string Word { get; init; }
        public required int Count { get; init; }
    }
}
