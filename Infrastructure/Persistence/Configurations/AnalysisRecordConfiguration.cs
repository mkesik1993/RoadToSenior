using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordAnalytics.Infrastructure.Persistence.Entities;

namespace WordAnalytics.Infrastructure.Persistence.Configurations;

public sealed class AnalysisRecordConfiguration : IEntityTypeConfiguration<AnalysisRecord>
{
    public void Configure(EntityTypeBuilder<AnalysisRecord> builder)
    {
        builder.ToTable("Analyses", table =>
            table.HasCheckConstraint("CK_Analyses_TopCount", "\"TopCount\" > 0"));

        builder.HasKey(analysis => analysis.Id);

        builder.Property(analysis => analysis.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(analysis => analysis.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(analysis => analysis.TopCount)
            .IsRequired();

        builder.HasMany(analysis => analysis.TopWords)
            .WithOne(word => word.Analysis)
            .HasForeignKey(word => word.AnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
