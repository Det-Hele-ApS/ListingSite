using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ListingApp.DataAccess.Migrations
{
    public partial class AddedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EscortId = table.Column<Guid>(nullable: false),
                    CityId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calendar_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calendar_Escorts_EscortId",
                        column: x => x.EscortId,
                        principalTable: "Escorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscortFeatures",
                columns: table => new
                {
                    FeatureName = table.Column<string>(nullable: false),
                    FeatureValue = table.Column<string>(nullable: false),
                    EscortId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscortFeatures", x => new { x.FeatureName, x.EscortId });
                    table.ForeignKey(
                        name: "FK_EscortFeatures_Escorts_EscortId",
                        column: x => x.EscortId,
                        principalTable: "Escorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    EscortId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Escorts_EscortId",
                        column: x => x.EscortId,
                        principalTable: "Escorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_CityId",
                table: "Calendar",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_EscortId",
                table: "Calendar",
                column: "EscortId");

            migrationBuilder.CreateIndex(
                name: "IX_EscortFeatures_EscortId",
                table: "EscortFeatures",
                column: "EscortId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_EscortId",
                table: "Images",
                column: "EscortId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calendar");

            migrationBuilder.DropTable(
                name: "EscortFeatures");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
