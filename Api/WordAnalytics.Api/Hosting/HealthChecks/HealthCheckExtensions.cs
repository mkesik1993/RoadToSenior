using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WordAnalytics.Api.Infrastructure.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck<WordAnalyticsHealthCheck>("word-analytics", tags: ["ready"]);

            return services;
        }

        public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("live"),
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            })
                .DisableRateLimiting();

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("ready"),
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            })
                .DisableRateLimiting();

            return app;
        }
    }
}
