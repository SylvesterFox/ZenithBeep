using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLobbyKeyDel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomersLobbies",
                table: "RoomersLobbies");

            migrationBuilder.DropColumn(
                name: "keyId",
                table: "RoomersLobbies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomersLobbies",
                table: "RoomersLobbies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms",
                column: "LobbyId",
                principalTable: "RoomersLobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomersLobbies",
                table: "RoomersLobbies");

            migrationBuilder.AddColumn<decimal>(
                name: "keyId",
                table: "RoomersLobbies",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomersLobbies",
                table: "RoomersLobbies",
                column: "keyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                table: "ItemsRooms",
                column: "LobbyId",
                principalTable: "RoomersLobbies",
                principalColumn: "keyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
