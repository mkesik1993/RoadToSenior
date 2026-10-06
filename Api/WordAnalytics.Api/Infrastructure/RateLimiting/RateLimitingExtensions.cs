using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.RateLimiting;

namespace WordAnalytics.Api.Infrastructure.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RateLimitingSettings>()
                .Bind(configuration.GetSection(RateLimitingSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(RateLimitingSettings.AnalyzePolicy, httpContext =>
                {
                    var settings = httpContext.RequestServices
                        .GetRequiredService<IOptions<RateLimitingSettings>>().Value;

                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = settings.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
                    }

                    var problemDetailsService = context.HttpContext.RequestServices
                        .GetRequiredService<IProblemDetailsService>();

                    await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Title = "Too many requests.",
                            Status = StatusCodes.Status429TooManyRequests,
                            Detail = "Request limit exceeded. Try again later.",
                            Type = "https://tools.ietf.org/html/rfc6585#section-4"
                        }
                    });
                };
            });

            return services;
        }
    }
}
