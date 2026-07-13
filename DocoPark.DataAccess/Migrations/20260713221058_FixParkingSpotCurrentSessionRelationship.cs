using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocoPark.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixParkingSpotCurrentSessionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId1",
                table: "ParkingSpots");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpots_CurrentSessionId1",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "CurrentSessionId1",
                table: "ParkingSpots");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpots_ParkingSessions_CurrentSessionId",
                table: "ParkingSpots");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpots_CurrentSessionId",
                table: "ParkingSpots");

            migrationBuilder.AddColumn<int>(
                name: "CurrentSessionId1",
                table: "ParkingSpots",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 1,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 2,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 3,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 4,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 5,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 6,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 7,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 8,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 9,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 10,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 11,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 12,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 13,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 14,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 15,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 16,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 17,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 18,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 19,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 20,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 21,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 22,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 23,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 24,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 25,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 26,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 27,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 28,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 29,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 30,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 31,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 32,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 33,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 34,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 35,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 36,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 37,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 38,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 39,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 40,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 41,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 42,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 43,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 44,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 45,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 46,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 47,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 48,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 49,
                column: "CurrentSessionId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "ParkingSpots",
                keyColumn: "Id",
                keyValue: 50,
                column: "CurrentSessionId1",
                value: null);

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
    }
}
