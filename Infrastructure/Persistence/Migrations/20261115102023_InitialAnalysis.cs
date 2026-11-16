using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordAnalytics.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TopCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.CheckConstraint("CK_Analyses_TopCount", "\"TopCount\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "AnalysisWords",
                columns: table => new
                {
                    AnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Word = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisWords", x => new { x.AnalysisId, x.Position });
                    table.CheckConstraint("CK_AnalysisWords_Count", "\"Count\" > 0");
                    table.CheckConstraint("CK_AnalysisWords_Position", "\"Position\" > 0");
                    table.ForeignKey(
                        name: "FK_AnalysisWords_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisWords_AnalysisId_Word",
                table: "AnalysisWords",
                columns: new[] { "AnalysisId", "Word" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisWords");

            migrationBuilder.DropTable(
                name: "Analyses");
        }
    }
}
