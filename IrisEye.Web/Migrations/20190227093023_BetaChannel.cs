using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class BetaChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BetaChannel",
                table: "Runs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetaChannel",
                table: "Runs");
        }
    }
}
