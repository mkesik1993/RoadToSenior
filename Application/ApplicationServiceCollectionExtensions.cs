using Microsoft.Extensions.DependencyInjection;
using WordAnalytics.Application.Features.AnalyzeText;

namespace WordAnalytics.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<AnalyzeTextHandler>();
        return services;
    }
}
