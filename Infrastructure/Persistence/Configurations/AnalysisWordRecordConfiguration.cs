using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordAnalytics.Infrastructure.Persistence.Entities;

namespace WordAnalytics.Infrastructure.Persistence.Configurations;

public sealed class AnalysisWordRecordConfiguration : IEntityTypeConfiguration<AnalysisWordRecord>
{
    public void Configure(EntityTypeBuilder<AnalysisWordRecord> builder)
    {
        builder.ToTable("AnalysisWords", table =>
        {
            table.HasCheckConstraint("CK_AnalysisWords_Position", "\"Position\" > 0");
            table.HasCheckConstraint("CK_AnalysisWords_Count", "\"Count\" > 0");
        });

        builder.HasKey(word => new { word.AnalysisId, word.Position });

        builder.Property(word => word.Word)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(word => word.Count)
            .IsRequired();

        builder.HasIndex(word => new { word.AnalysisId, word.Word })
            .IsUnique();
    }
}
