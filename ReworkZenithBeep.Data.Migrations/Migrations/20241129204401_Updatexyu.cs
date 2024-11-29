using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Updatexyu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomersLobbies_Guilds_LobbyId",
                table: "RoomersLobbies");

            migrationBuilder.DropIndex(
                name: "IX_RoomersLobbies_LobbyId",
                table: "RoomersLobbies");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomersLobbies_Guilds_Id",
                table: "RoomersLobbies",
                column: "Id",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomersLobbies_Guilds_Id",
                table: "RoomersLobbies");

            migrationBuilder.CreateIndex(
                name: "IX_RoomersLobbies_LobbyId",
                table: "RoomersLobbies",
                column: "LobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomersLobbies_Guilds_LobbyId",
                table: "RoomersLobbies",
                column: "LobbyId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
