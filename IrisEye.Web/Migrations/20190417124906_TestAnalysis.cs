using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IrisEye.Web.Migrations
{
    public partial class TestAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AnalysisResultId",
                table: "Tests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnalysisResult",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    IdentifiedAtId = table.Column<long>(nullable: true),
                    ById = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    AnalysisStatus = table.Column<int>(nullable: false),
                    StartedOn = table.Column<DateTime>(nullable: false),
                    FinishedOn = table.Column<DateTime>(nullable: false),
                    GitHubId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisResult_Users_ById",
                        column: x => x.ById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnalysisResult_Steps_IdentifiedAtId",
                        column: x => x.IdentifiedAtId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tests_AnalysisResultId",
                table: "Tests",
                column: "AnalysisResultId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResult_ById",
                table: "AnalysisResult",
                column: "ById");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResult_IdentifiedAtId",
                table: "AnalysisResult",
                column: "IdentifiedAtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AnalysisResult_AnalysisResultId",
                table: "Tests",
                column: "AnalysisResultId",
                principalTable: "AnalysisResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AnalysisResult_AnalysisResultId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "AnalysisResult");

            migrationBuilder.DropIndex(
                name: "IX_Tests_AnalysisResultId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AnalysisResultId",
                table: "Tests");
        }
    }
}
