using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ListingApp.DataAccess.Migrations
{
    public partial class Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Services",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Cities",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Services_Slug",
                table: "Services",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Escorts_ExternalId",
                table: "Escorts",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Slug",
                table: "Cities",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_Slug",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Escorts_ExternalId",
                table: "Escorts");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Slug",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Services",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Cities",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
