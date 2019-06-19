using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IrisEye.Web.Migrations
{
    public partial class TestCaseComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "TestCases",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "AttentionRequired",
                table: "TestCases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TestCaseComments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Message = table.Column<string>(nullable: false),
                    TestCaseId = table.Column<long>(nullable: true),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    AddedById = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseComments_Users_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCaseComments_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseComments_AddedById",
                table: "TestCaseComments",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseComments_TestCaseId",
                table: "TestCaseComments",
                column: "TestCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseComments");

            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "AttentionRequired",
                table: "TestCases");
        }
    }
}
