using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class TestAnalysisReferenced : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResult_Users_ById",
                table: "AnalysisResult");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResult_Steps_IdentifiedAtId",
                table: "AnalysisResult");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AnalysisResult_AnalysisResultId",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalysisResult",
                table: "AnalysisResult");

            migrationBuilder.RenameTable(
                name: "AnalysisResult",
                newName: "AnalysisResults");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResult_IdentifiedAtId",
                table: "AnalysisResults",
                newName: "IX_AnalysisResults_IdentifiedAtId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResult_ById",
                table: "AnalysisResults",
                newName: "IX_AnalysisResults_ById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_Users_ById",
                table: "AnalysisResults",
                column: "ById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_Steps_IdentifiedAtId",
                table: "AnalysisResults",
                column: "IdentifiedAtId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AnalysisResults_AnalysisResultId",
                table: "Tests",
                column: "AnalysisResultId",
                principalTable: "AnalysisResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_Users_ById",
                table: "AnalysisResults");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_Steps_IdentifiedAtId",
                table: "AnalysisResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AnalysisResults_AnalysisResultId",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults");

            migrationBuilder.RenameTable(
                name: "AnalysisResults",
                newName: "AnalysisResult");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResults_IdentifiedAtId",
                table: "AnalysisResult",
                newName: "IX_AnalysisResult_IdentifiedAtId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResults_ById",
                table: "AnalysisResult",
                newName: "IX_AnalysisResult_ById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalysisResult",
                table: "AnalysisResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResult_Users_ById",
                table: "AnalysisResult",
                column: "ById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResult_Steps_IdentifiedAtId",
                table: "AnalysisResult",
                column: "IdentifiedAtId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AnalysisResult_AnalysisResultId",
                table: "Tests",
                column: "AnalysisResultId",
                principalTable: "AnalysisResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
