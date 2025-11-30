using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToTriNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM");

            migrationBuilder.RenameTable(
                name: "StationTriNM",
                newName: "StationTriNMTriNM");

            migrationBuilder.RenameTable(
                name: "ChargerTriNM",
                newName: "ChargerTriNMTriNM");

            migrationBuilder.RenameIndex(
                name: "IX_ChargerTriNM_StationTriNMID",
                table: "ChargerTriNMTriNM",
                newName: "IX_ChargerTriNMTriNM_StationTriNMID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNMTriNM",
                table: "StationTriNMTriNM",
                column: "StationTriNMID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNMTriNM",
                table: "ChargerTriNMTriNM",
                column: "ChargerTriNMID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNMTriNM",
                table: "StationTriNMTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNMTriNM",
                table: "ChargerTriNMTriNM");

            migrationBuilder.RenameTable(
                name: "StationTriNMTriNM",
                newName: "StationTriNM");

            migrationBuilder.RenameTable(
                name: "ChargerTriNMTriNM",
                newName: "ChargerTriNM");

            migrationBuilder.RenameIndex(
                name: "IX_ChargerTriNMTriNM_StationTriNMID",
                table: "ChargerTriNM",
                newName: "IX_ChargerTriNM_StationTriNMID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM",
                column: "StationTriNMID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM",
                column: "ChargerTriNMID");
        }
    }
}
