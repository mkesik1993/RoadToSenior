using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WordAnalytics.Application.AnalysisHistory;
using WordAnalytics.Infrastructure.AnalysisHistory;
using WordAnalytics.Infrastructure.Persistence;

namespace WordAnalytics.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WordAnalytics");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:WordAnalytics must be configured to use Infrastructure services.");
        }

        services.AddDbContext<AnalysisDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAnalysisStore, EfAnalysisStore>();

        return services;
    }
}
