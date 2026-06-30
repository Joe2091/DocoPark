using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocoPark.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVehicleTypeFromParkingSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId",
                table: "ParkingSpots");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpots_CurrentSessionId",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "IsOccupied",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "ParkingSpots");

            migrationBuilder.AddColumn<int>(
                name: "CurrentSessionId1",
                table: "ParkingSpots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotStatus",
                table: "ParkingSpots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "ParkingSpots",
                columns: new[] { "Id", "CurrentSessionId", "CurrentSessionId1", "SpotNumber", "SpotStatus" },
                values: new object[,]
                {
                    { 1, null, null, "S001", "Available" },
                    { 2, null, null, "S002", "Available" },
                    { 3, null, null, "S003", "Available" },
                    { 4, null, null, "S004", "Available" },
                    { 5, null, null, "S005", "Available" },
                    { 6, null, null, "S006", "Available" },
                    { 7, null, null, "S007", "Available" },
                    { 8, null, null, "S008", "Available" },
                    { 9, null, null, "S009", "Available" },
                    { 10, null, null, "S010", "Available" },
                    { 11, null, null, "S011", "Available" },
                    { 12, null, null, "S012", "Available" },
                    { 13, null, null, "S013", "Available" },
                    { 14, null, null, "S014", "Available" },
                    { 15, null, null, "S015", "Available" },
                    { 16, null, null, "S016", "Available" },
                    { 17, null, null, "S017", "Available" },
                    { 18, null, null, "S018", "Available" },
                    { 19, null, null, "S019", "Available" },
                    { 20, null, null, "S020", "Available" },
                    { 21, null, null, "S021", "Available" },
                    { 22, null, null, "S022", "Available" },
                    { 23, null, null, "S023", "Available" },
                    { 24, null, null, "S024", "Available" },
                    { 25, null, null, "S025", "Available" },
                    { 26, null, null, "S026", "Available" },
                    { 27, null, null, "S027", "Available" },
                    { 28, null, null, "S028", "Available" },
                    { 29, null, null, "S029", "Available" },
                    { 30, null, null, "S030", "Available" },
                    { 31, null, null, "S031", "Available" },
                    { 32, null, null, "S032", "Available" },
                    { 33, null, null, "S033", "Available" },
                    { 34, null, null, "S034", "Available" },
                    { 35, null, null, "S035", "Available" },
                    { 36, null, null, "S036", "Available" },
                    { 37, null, null, "S037", "Available" },
                    { 38, null, null, "S038", "Available" },
                    { 39, null, null, "S039", "Available" },
                    { 40, null, null, "S040", "Available" },
                    { 41, null, null, "S041", "Available" },
                    { 42, null, null, "S042", "Available" },
                    { 43, null, null, "S043", "Available" },
                    { 44, null, null, "S044", "Available" },
                    { 45, null, null, "S045", "Available" },
                    { 46, null, null, "S046", "Available" },
                    { 47, null, null, "S047", "Available" },
                    { 48, null, null, "S048", "Available" },
                    { 49, null, null, "S049", "Available" },
                    { 50, null, null, "S050", "Available" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_CurrentSessionId1",
                table: "ParkingSpots",
                column: "CurrentSessionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId1",
                table: "ParkingSpots",
                column: "CurrentSessionId1",
                principalTable: "ParkingSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId1",
                table: "ParkingSpots");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpots_CurrentSessionId1",
                table: "ParkingSpots");

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DropColumn(
                name: "CurrentSessionId1",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "SpotStatus",
                table: "ParkingSpots");

            migrationBuilder.AddColumn<bool>(
                name: "IsOccupied",
                table: "ParkingSpots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "ParkingSpots",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_CurrentSessionId",
                table: "ParkingSpots",
                column: "CurrentSessionId",
                unique: true,
                filter: "[CurrentSessionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId",
                table: "ParkingSpots",
                column: "CurrentSessionId",
                principalTable: "ParkingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
