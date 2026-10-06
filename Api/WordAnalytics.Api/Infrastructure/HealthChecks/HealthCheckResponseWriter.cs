using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WordAnalytics.Api.Infrastructure.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
        public static Task WriteAsync(HttpContext context, HealthReport report)
        {
            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds
                })
            };

            return context.Response.WriteAsJsonAsync(payload, context.RequestAborted);
        }
    }
}
