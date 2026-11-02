using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Filters;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Application.Features.CountWords;

namespace WordAnalytics.Api.Features.CountWords;

public static class CountWordsEndpoint
{
    public static IEndpointRouteBuilder MapCountWordsEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Analysis");

        group.MapPost("/word-counts", CountWords)
            .WithName("CountWords")
            .WithSummary("Returns all word frequencies in the given text.")
            .AddEndpointFilter<ValidationFilter<CountWordsRequest>>()
            .RequireRateLimiting(RateLimitingSettings.AnalyzePolicy)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static Ok<CountWordsResponse> CountWords(CountWordsRequest request, CountWordsHandler handler)
    {
        var result = handler.Handle(new CountWordsInput(request.Text));

        var response = new CountWordsResponse
        {
            Counts = result.Counts
                .Select(kv => new WordCountDto { Word = kv.Key, Count = kv.Value })
                .ToList()
        };

        return TypedResults.Ok(response);
    }
}
