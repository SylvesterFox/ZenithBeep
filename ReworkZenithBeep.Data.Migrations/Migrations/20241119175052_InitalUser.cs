using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReworkZenithBeep.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    cookieBox = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
