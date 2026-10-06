using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Contracts.Requests;
using WordAnalytics.Api.Contracts.Responses;
using WordAnalytics.Api.Filters;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Ranking.Interfaces;

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

        private static Ok<AnalyzeResponse> Analyze(AnalyzeRequest request, IWordCounter counter, IWordRanker ranker)
        {
            var counts = counter.CountWords(request.Text);
            var top = ranker.GetTop(counts, request.TopCount);

            var response = new AnalyzeResponse
            {
                Top = top
                    .Select(kv => new WordFrequencyDto { Word = kv.Key, Count = kv.Value })
                    .ToList()
            };

            return TypedResults.Ok(response);
        }
    }
}
