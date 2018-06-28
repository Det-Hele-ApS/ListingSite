using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ListingApp.DataAccess.Migrations
{
    public partial class RemoveCalendarIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Calendar_Date_CityId_EscortId",
                table: "Calendar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Calendar_Date_CityId_EscortId",
                table: "Calendar",
                columns: new[] { "Date", "CityId", "EscortId" },
                unique: true);
        }
    }
}
