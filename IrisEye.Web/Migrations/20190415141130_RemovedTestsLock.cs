using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class RemovedTestsLock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestInfos_Users_LockedById",
                table: "TestInfos");

            migrationBuilder.DropIndex(
                name: "IX_TestInfos_LockedById",
                table: "TestInfos");

            migrationBuilder.DropColumn(
                name: "LockedById",
                table: "TestInfos");

            migrationBuilder.DropColumn(
                name: "LockedOn",
                table: "TestInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LockedById",
                table: "TestInfos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedOn",
                table: "TestInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TestInfos_LockedById",
                table: "TestInfos",
                column: "LockedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TestInfos_Users_LockedById",
                table: "TestInfos",
                column: "LockedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
