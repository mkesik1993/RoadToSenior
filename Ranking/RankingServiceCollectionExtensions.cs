using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WordAnalytics.Ranking.Interfaces;
using WordAnalytics.Ranking.Services;
using WordAnalytics.Ranking.Settings;

namespace WordAnalytics.Ranking
{
    public static class RankingServiceCollectionExtensions
    {
        public static IServiceCollection AddWordRanking(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RankingOptions>()
                .Bind(configuration.GetSection(RankingOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IWordRanker, WordRanker>();
            return services;
        }
    }
}
