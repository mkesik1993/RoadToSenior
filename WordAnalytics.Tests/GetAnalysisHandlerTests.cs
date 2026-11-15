using WordAnalytics.Application.Features.GetAnalysis;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class GetAnalysisHandlerTests
{
    private static readonly DateTimeOffset CreatedAtUtc = new(2026, 11, 12, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task HandleAsync_WithExistingAnalysis_ReturnsStoredDetails()
    {
        // Arrange
        var analysis = CreateAnalysis();
        var store = new FakeAnalysisStore();
        await store.AddAsync(analysis, CancellationToken.None);
        var handler = new GetAnalysisHandler(store);

        // Act
        var result = await handler.HandleAsync(new GetAnalysisInput(analysis.Id));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(analysis.Id, result.Id);
        Assert.Equal(analysis.Text, result.Text);
        Assert.Equal(analysis.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(analysis.TopCount, result.TopCount);
        Assert.Equal(analysis.TopWords, result.TopWords);
    }

    [Fact]
    public async Task HandleAsync_WithMissingAnalysis_ReturnsNull()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = new GetAnalysisHandler(store);

        // Act
        var result = await handler.HandleAsync(new GetAnalysisInput(Guid.NewGuid()));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_PassesTokenToStore()
    {
        // Arrange
        var store = new FakeAnalysisStore();
        var handler = new GetAnalysisHandler(store);
        using var cancellationSource = new CancellationTokenSource();
        var input = new GetAnalysisInput(Guid.NewGuid());

        // Act
        await handler.HandleAsync(input, cancellationSource.Token);

        // Assert
        Assert.Equal(cancellationSource.Token, store.GetCancellationToken);
    }

    private static TextAnalysis CreateAnalysis()
    {
        return TextAnalysis.Create(
            Guid.NewGuid(),
            "hello hello world",
            CreatedAtUtc,
            2,
            [
                new KeyValuePair<string, int>("hello", 2),
                new("world", 1)
            ]);
    }
}
