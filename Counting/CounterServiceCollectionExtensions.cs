using Microsoft.Extensions.DependencyInjection;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Counting.Services;

namespace WordAnalytics.Counting
{
    public static class CountingServiceCollectionExtensions
    {
        public static IServiceCollection AddWordCounting(this IServiceCollection services)
        {
            services.AddSingleton<IWordCounter, WordCounter>();
            return services;
        }
    }
}
