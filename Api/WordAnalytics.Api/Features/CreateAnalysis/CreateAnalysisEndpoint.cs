using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Features.Analyses;
using WordAnalytics.Api.Features.GetAnalysis;
using WordAnalytics.Api.Filters;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Application.Features.CreateAnalysis;

namespace WordAnalytics.Api.Features.CreateAnalysis;

public static class CreateAnalysisEndpoint
{
    public static IEndpointRouteBuilder MapCreateAnalysisEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analyses")
            .WithTags("Analyses");

        group.MapPost("", CreateAnalysis)
            .WithName("CreateAnalysis")
            .WithSummary("Analyzes text and stores the result.")
            .AddEndpointFilter<ValidationFilter<CreateAnalysisRequest>>()
            .RequireRateLimiting(RateLimitingSettings.AnalyzePolicy)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<CreatedAtRoute<AnalysisResponse>> CreateAnalysis(CreateAnalysisRequest request, CreateAnalysisHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CreateAnalysisInput(request.Text, request.TopCount), cancellationToken);

        var response = new AnalysisResponse
        {
            Id = result.Id,
            Text = result.Text,
            CreatedAtUtc = result.CreatedAtUtc,
            TopCount = result.TopCount,
            TopWords = result.TopWords
                .Select(word => new AnalysisWordDto { Word = word.Key, Count = word.Value })
                .ToList()
        };

        return TypedResults.CreatedAtRoute(response, GetAnalysisEndpoint.RouteName, new { id = response.Id });
    }
}
