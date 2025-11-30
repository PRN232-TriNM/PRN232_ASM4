using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddImageURLToChargerTriNMAndStationTriNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "StationTriNM",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "ChargerTriNM",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "StationTriNM");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "ChargerTriNM");
        }
    }
}
