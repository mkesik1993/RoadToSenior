using Microsoft.EntityFrameworkCore;
using WordAnalytics.Infrastructure.Persistence.Entities;

namespace WordAnalytics.Infrastructure.Persistence;

public class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : DbContext(options)
{
    public DbSet<AnalysisRecord> Analyses => Set<AnalysisRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalysisDbContext).Assembly);
    }
}
