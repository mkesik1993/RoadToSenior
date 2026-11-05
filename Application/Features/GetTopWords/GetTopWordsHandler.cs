using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application.Features.GetTopWords;

public class GetTopWordsHandler(IWordRanker ranker, IOptions<RankingOptions> options)
{
    private readonly RankingOptions _options = options.Value;

    public GetTopWordsResult Handle(GetTopWordsInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var counts = input.Counts.ToDictionary(kv => kv.Key, kv => kv.Value);

        return new GetTopWordsResult(ranker.GetTop(counts, input.TopCount ?? _options.TopCount));
    }
}
