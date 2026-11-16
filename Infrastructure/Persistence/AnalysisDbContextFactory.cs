using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WordAnalytics.Infrastructure.Persistence;

public sealed class AnalysisDbContextFactory : IDesignTimeDbContextFactory<AnalysisDbContext>
{
    public AnalysisDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__WordAnalytics");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set the ConnectionStrings__WordAnalytics environment variable for EF Core tooling.");
        }

        var options = new DbContextOptionsBuilder<AnalysisDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AnalysisDbContext(options);
    }
}
