using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ListingApp.DataAccess.Migrations
{
    public partial class ExternalLinksForSyncData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLink",
                table: "Images",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmallPath",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "EscortFeatures",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLink",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "SmallPath",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "EscortFeatures");
        }
    }
}
