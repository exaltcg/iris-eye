using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IrisEye.Web.Migrations
{
    public partial class Analysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StepAnalysisItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ParentStepId = table.Column<long>(nullable: false),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    AnalysisResolution = table.Column<int>(nullable: false),
                    AddedById = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    GitHubId = table.Column<int>(nullable: false),
                    BugzillaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepAnalysisItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepAnalysisItems_Users_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepAnalysisItems_Steps_ParentStepId",
                        column: x => x.ParentStepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepAnalysisItems_AddedById",
                table: "StepAnalysisItems",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_StepAnalysisItems_ParentStepId",
                table: "StepAnalysisItems",
                column: "ParentStepId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepAnalysisItems");
        }
    }
}
