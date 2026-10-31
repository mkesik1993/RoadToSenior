using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Contracts.Requests;
using WordAnalytics.Api.Contracts.Responses;
using WordAnalytics.Api.Filters;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Application.Features.AnalyzeText;

namespace WordAnalytics.Api.Controllers
{
    public static class AnalysisController
    {
        public static IEndpointRouteBuilder MapAnalysisEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api")
                .WithTags("Analysis");

            group.MapPost("/analyze", Analyze)
                .WithName("AnalyzeText")
                .WithSummary("Returns the most frequently used words in the given text.")
                .AddEndpointFilter<ValidationFilter<AnalyzeRequest>>()
                .RequireRateLimiting(RateLimitingSettings.AnalyzePolicy)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status429TooManyRequests);

            return app;
        }

        private static Ok<AnalyzeResponse> Analyze(AnalyzeRequest request, AnalyzeTextHandler handler)
        {
            var result = handler.Handle(new AnalyzeTextInput(request.Text, request.TopCount));

            var response = new AnalyzeResponse
            {
                Top = result.Top
                    .Select(kv => new WordFrequencyDto { Word = kv.Key, Count = kv.Value })
                    .ToList()
            };

            return TypedResults.Ok(response);
        }
    }
}
