﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace IrisEye.Web.Migrations
{
    public partial class TestCaseCommentsunread : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "TestCaseComments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "TestCaseComments");
        }
    }
}
