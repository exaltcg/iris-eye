using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class IssueMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TestCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "TestCases");
        }
    }
}
