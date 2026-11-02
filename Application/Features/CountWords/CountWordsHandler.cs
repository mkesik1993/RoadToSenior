using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application.Features.CountWords;

public class CountWordsHandler(IWordCounter counter)
{
    public CountWordsResult Handle(CountWordsInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new CountWordsResult(counter.CountWords(input.Text).ToList());
    }
}
