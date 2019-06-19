using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class newfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuiteName",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "Runs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuiteName",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Runs");
        }
    }
}
