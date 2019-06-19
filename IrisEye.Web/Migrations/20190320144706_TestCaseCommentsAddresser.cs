using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class TestCaseCommentsAddresser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AddressedToId",
                table: "TestCaseComments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseComments_AddressedToId",
                table: "TestCaseComments",
                column: "AddressedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseComments_Users_AddressedToId",
                table: "TestCaseComments",
                column: "AddressedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCaseComments_Users_AddressedToId",
                table: "TestCaseComments");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseComments_AddressedToId",
                table: "TestCaseComments");

            migrationBuilder.DropColumn(
                name: "AddressedToId",
                table: "TestCaseComments");
        }
    }
}
