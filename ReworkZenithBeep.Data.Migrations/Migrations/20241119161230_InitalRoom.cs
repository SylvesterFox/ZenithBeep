using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitalRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomersLobbies",
                columns: table => new
                {
                    keyId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LobbyId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomersLobbies", x => x.keyId);
                    table.ForeignKey(
                        name: "FK_RoomersLobbies_Guilds_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsRooms",
                columns: table => new
                {
                    userId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LobbyId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    nameChannel = table.Column<string>(type: "text", nullable: false),
                    limitChannel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsRooms", x => x.userId);
                    table.ForeignKey(
                        name: "FK_ItemsRooms_RoomersLobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "RoomersLobbies",
                        principalColumn: "keyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsRooms_LobbyId",
                table: "ItemsRooms",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomersLobbies_LobbyId",
                table: "RoomersLobbies",
                column: "LobbyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsRooms");

            migrationBuilder.DropTable(
                name: "RoomersLobbies");
        }
    }
}
