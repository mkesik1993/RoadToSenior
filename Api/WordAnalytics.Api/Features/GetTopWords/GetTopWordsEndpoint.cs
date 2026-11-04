using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Filters;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Application.Features.GetTopWords;

namespace WordAnalytics.Api.Features.GetTopWords;

public static class GetTopWordsEndpoint
{
    public static IEndpointRouteBuilder MapGetTopWordsEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Analysis");

        group.MapPost("/top-words", GetTopWords)
            .WithName("GetTopWords")
            .WithSummary("Returns the most frequent words from the supplied word counts.")
            .AddEndpointFilter<ValidationFilter<GetTopWordsRequest>>()
            .RequireRateLimiting(RateLimitingSettings.AnalyzePolicy)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static Ok<GetTopWordsResponse> GetTopWords(GetTopWordsRequest request, GetTopWordsHandler handler)
    {
        var counts = request.Counts!
            .Select(item => new KeyValuePair<string, int>(item.Word!, item.Count))
            .ToList();

        var result = handler.Handle(new GetTopWordsInput(counts, request.TopCount));

        var response = new GetTopWordsResponse
        {
            Top = result.Top
                .Select(kv => new TopWordDto { Word = kv.Key, Count = kv.Value })
                .ToList()
        };

        return TypedResults.Ok(response);
    }
}
