using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCS.TriNM.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RenamePrimaryKeysToTriNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_ChargerTriNM_StationTriNM",
                table: "ChargerTriNMTriNM");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ChargerTriNM",
                table: "ChargingTransaction");

            // Drop primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNMTriNM",
                table: "StationTriNMTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNMTriNM",
                table: "ChargerTriNMTriNM");

            // Rename tables
            migrationBuilder.RenameTable(
                name: "StationTriNMTriNM",
                newName: "StationTriNM");

            migrationBuilder.RenameTable(
                name: "ChargerTriNMTriNM",
                newName: "ChargerTriNM");

            // Rename columns
            migrationBuilder.RenameColumn(
                name: "StationTriNMID",
                table: "StationTriNM",
                newName: "StationTriNMTriNMId");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMID",
                table: "ChargerTriNM",
                newName: "ChargerTriNMTriNMId");

            migrationBuilder.RenameColumn(
                name: "StationTriNMID",
                table: "ChargerTriNM",
                newName: "StationTriNMTriNMId");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMID",
                table: "ChargingTransaction",
                newName: "ChargerTriNMTriNMId");

            // Drop and recreate indexes
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChargerTriNMTriNM_StationTriNMID' AND object_id = OBJECT_ID('ChargerTriNM'))
                    DROP INDEX [IX_ChargerTriNMTriNM_StationTriNMID] ON [ChargerTriNM];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ChargerTriNM_StationTriNMTriNMId",
                table: "ChargerTriNM",
                column: "StationTriNMTriNMId");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChargingTransaction_ChargerTriNMID' AND object_id = OBJECT_ID('ChargingTransaction'))
                    DROP INDEX [IX_ChargingTransaction_ChargerTriNMID] ON [ChargingTransaction];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingTransaction_ChargerTriNMTriNMId",
                table: "ChargingTransaction",
                column: "ChargerTriNMTriNMId");

            // Add primary keys back
            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM",
                column: "StationTriNMTriNMId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM",
                column: "ChargerTriNMTriNMId");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_ChargerTriNM_StationTriNM",
                table: "ChargerTriNM",
                column: "StationTriNMTriNMId",
                principalTable: "StationTriNM",
                principalColumn: "StationTriNMTriNMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ChargerTriNM",
                table: "ChargingTransaction",
                column: "ChargerTriNMTriNMId",
                principalTable: "ChargerTriNM",
                principalColumn: "ChargerTriNMTriNMId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_ChargerTriNM_StationTriNM",
                table: "ChargerTriNM");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ChargerTriNM",
                table: "ChargingTransaction");

            // Drop primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationTriNM",
                table: "StationTriNM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargerTriNM",
                table: "ChargerTriNM");

            // Rename columns back
            migrationBuilder.RenameColumn(
                name: "StationTriNMTriNMId",
                table: "StationTriNM",
                newName: "StationTriNMID");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMTriNMId",
                table: "ChargerTriNM",
                newName: "ChargerTriNMID");

            migrationBuilder.RenameColumn(
                name: "StationTriNMTriNMId",
                table: "ChargerTriNM",
                newName: "StationTriNMID");

            migrationBuilder.RenameColumn(
                name: "ChargerTriNMTriNMId",
                table: "ChargingTransaction",
                newName: "ChargerTriNMID");

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_ChargerTriNM_StationTriNMTriNMId",
                table: "ChargerTriNM",
                newName: "IX_ChargerTriNMTriNM_StationTriNMID");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingTransaction_ChargerTriNMTriNMId",
                table: "ChargingTransaction",
                newName: "IX_ChargingTransaction_ChargerTriNMID");

            // Rename tables back
            migrationBuilder.RenameTable(
                name: "StationTriNM",
                newName: "StationTriNMTriNM");

            migrationBuilder.RenameTable(
                name: "ChargerTriNM",
                newName: "ChargerTriNMTriNM");

            // Add primary keys back
            migrationBuilder.AddPrimaryKey(
                name: "PK_StationTriNMTriNM",
                table: "StationTriNMTriNM",
                column: "StationTriNMID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargerTriNMTriNM",
                table: "ChargerTriNMTriNM",
                column: "ChargerTriNMID");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_ChargerTriNM_StationTriNM",
                table: "ChargerTriNMTriNM",
                column: "StationTriNMID",
                principalTable: "StationTriNMTriNM",
                principalColumn: "StationTriNMID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ChargerTriNM",
                table: "ChargingTransaction",
                column: "ChargerTriNMID",
                principalTable: "ChargerTriNMTriNM",
                principalColumn: "ChargerTriNMID");
        }
    }
}
