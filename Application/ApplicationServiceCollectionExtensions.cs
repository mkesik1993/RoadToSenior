using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Application.Features.CountWords;
using WordAnalytics.Application.Features.GetTopWords;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RankingOptions>()
            .Bind(configuration.GetSection(RankingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IWordCounter, WordCounter>();
        services.AddSingleton<IWordRanker, WordRanker>();
        services.AddSingleton<AnalyzeTextHandler>();
        services.AddSingleton<CountWordsHandler>();
        services.AddSingleton<GetTopWordsHandler>();
        return services;
    }
}
