using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitalRoomTemp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemsTempRooms",
                columns: table => new
                {
                    ownerUser = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    roomid = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsTempRooms", x => x.ownerUser);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsTempRooms");
        }
    }
}
