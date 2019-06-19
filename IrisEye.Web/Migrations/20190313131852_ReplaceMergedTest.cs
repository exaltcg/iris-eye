using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class ReplaceMergedTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Merged",
                table: "TestCases");

            migrationBuilder.AddColumn<DateTime>(
                name: "MergedDate",
                table: "TestCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MergedDate",
                table: "TestCases");

            migrationBuilder.AddColumn<bool>(
                name: "Merged",
                table: "TestCases",
                nullable: false,
                defaultValue: false);
        }
    }
}
