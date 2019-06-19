using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class removedBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Runs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Runs",
                nullable: false,
                defaultValue: "");
        }
    }
}
