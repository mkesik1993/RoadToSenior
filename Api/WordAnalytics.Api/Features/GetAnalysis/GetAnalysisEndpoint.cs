using Microsoft.AspNetCore.Http.HttpResults;
using WordAnalytics.Api.Features.Analyses;
using WordAnalytics.Application.Features.GetAnalysis;

namespace WordAnalytics.Api.Features.GetAnalysis;

public static class GetAnalysisEndpoint
{
    public const string RouteName = "GetAnalysisById";

    public static IEndpointRouteBuilder MapGetAnalysisEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analyses")
            .WithTags("Analyses");

        group.MapGet("/{id:guid}", GetAnalysis)
            .WithName(RouteName)
            .WithSummary("Returns a previously stored text analysis.");

        return app;
    }

    private static async Task<Results<Ok<AnalysisResponse>, NotFound>> GetAnalysis(Guid id, GetAnalysisHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAnalysisInput(id), cancellationToken);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

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

        return TypedResults.Ok(response);
    }
}
