using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class TestCasesAndSteps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_Users_AssigneeId",
                table: "TestCase");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_Users_ReviewerId",
                table: "TestCase");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_TestSuites_TestSuiteId",
                table: "TestCase");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStep_TestCase_TestCaseId",
                table: "TestStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestStep",
                table: "TestStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase");

            migrationBuilder.RenameTable(
                name: "TestStep",
                newName: "TestSteps");

            migrationBuilder.RenameTable(
                name: "TestCase",
                newName: "TestCases");

            migrationBuilder.RenameIndex(
                name: "IX_TestStep_TestCaseId",
                table: "TestSteps",
                newName: "IX_TestSteps_TestCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCase_TestSuiteId",
                table: "TestCases",
                newName: "IX_TestCases_TestSuiteId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCase_ReviewerId",
                table: "TestCases",
                newName: "IX_TestCases_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCase_AssigneeId",
                table: "TestCases",
                newName: "IX_TestCases_AssigneeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestSteps",
                table: "TestSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Users_AssigneeId",
                table: "TestCases",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Users_ReviewerId",
                table: "TestCases",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_TestSuites_TestSuiteId",
                table: "TestCases",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSteps_TestCases_TestCaseId",
                table: "TestSteps",
                column: "TestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Users_AssigneeId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Users_ReviewerId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_TestSuites_TestSuiteId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSteps_TestCases_TestCaseId",
                table: "TestSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestSteps",
                table: "TestSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases");

            migrationBuilder.RenameTable(
                name: "TestSteps",
                newName: "TestStep");

            migrationBuilder.RenameTable(
                name: "TestCases",
                newName: "TestCase");

            migrationBuilder.RenameIndex(
                name: "IX_TestSteps_TestCaseId",
                table: "TestStep",
                newName: "IX_TestStep_TestCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_TestSuiteId",
                table: "TestCase",
                newName: "IX_TestCase_TestSuiteId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_ReviewerId",
                table: "TestCase",
                newName: "IX_TestCase_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_AssigneeId",
                table: "TestCase",
                newName: "IX_TestCase_AssigneeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestStep",
                table: "TestStep",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_Users_AssigneeId",
                table: "TestCase",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_Users_ReviewerId",
                table: "TestCase",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_TestSuites_TestSuiteId",
                table: "TestCase",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStep_TestCase_TestCaseId",
                table: "TestStep",
                column: "TestCaseId",
                principalTable: "TestCase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
