using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IrisEye.Web.Migrations
{
    public partial class TestCasesHistoryItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestCaseHistoryItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    AuthorId = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    TestCaseId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseHistoryItems_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCaseHistoryItems_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseHistoryItems_AuthorId",
                table: "TestCaseHistoryItems",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseHistoryItems_TestCaseId",
                table: "TestCaseHistoryItems",
                column: "TestCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseHistoryItems");
        }
    }
}
