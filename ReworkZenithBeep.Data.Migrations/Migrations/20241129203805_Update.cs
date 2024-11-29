using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms");

            migrationBuilder.DropIndex(
                name: "IX_ItemsRooms_LobbyId",
                table: "ItemsRooms");

            migrationBuilder.AddColumn<decimal>(
                name: "ItemRoomersLobbyId",
                table: "ItemsRooms",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ItemsRooms_ItemRoomersLobbyId",
                table: "ItemsRooms",
                column: "ItemRoomersLobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_ItemRoomersLobbyId",
                table: "ItemsRooms",
                column: "ItemRoomersLobbyId",
                principalTable: "RoomersLobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_ItemRoomersLobbyId",
                table: "ItemsRooms");

            migrationBuilder.DropIndex(
                name: "IX_ItemsRooms_ItemRoomersLobbyId",
                table: "ItemsRooms");

            migrationBuilder.DropColumn(
                name: "ItemRoomersLobbyId",
                table: "ItemsRooms");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsRooms_LobbyId",
                table: "ItemsRooms",
                column: "LobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms",
                column: "LobbyId",
                principalTable: "RoomersLobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
