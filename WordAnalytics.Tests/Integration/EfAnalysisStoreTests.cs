using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using WordAnalytics.Domain.Analysis;
using WordAnalytics.Infrastructure.AnalysisHistory;
using WordAnalytics.Infrastructure.Persistence;

namespace WordAnalytics.Tests.Integration;

public sealed class EfAnalysisStoreTests : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private AnalysisDbContext _dbContext = null!;
    private EfAnalysisStore _store = null!;

    public async Task InitializeAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("WORDANALYTICS_TEST_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _postgres = new PostgreSqlBuilder("postgres:17-alpine")
                .WithDatabase("wordanalytics")
                .WithUsername("wordanalytics")
                .WithPassword("wordanalytics-local-only")
                .Build();

            await _postgres.StartAsync();
            connectionString = _postgres.GetConnectionString();
        }

        var dbContextOptions = new DbContextOptionsBuilder<AnalysisDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AnalysisDbContext(dbContextOptions);
        await _dbContext.Database.MigrateAsync();

        _store = new EfAnalysisStore(_dbContext);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();

        if (_postgres is not null)
        {
            await _postgres.DisposeAsync();
        }
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_PreservesAnalysisAndWordOrder()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string text = "World, hello HELLO!";
        var createdAtUtc = new DateTimeOffset(2026, 11, 12, 8, 0, 0, TimeSpan.Zero);
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", 2),
            new KeyValuePair<string, int>("world", 1)
        };
        var analysis = TextAnalysis.Create(id, text, createdAtUtc, 2, topWords);

        // Act
        await _store.AddAsync(analysis, CancellationToken.None);
        var result = await _store.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(text, result.Text);
        Assert.Equal(createdAtUtc, result.CreatedAtUtc);
        Assert.Equal(2, result.TopCount);
        Assert.Equal(topWords, result.TopWords);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _store.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
