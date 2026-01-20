using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingsCup.API.Migrations
{
    /// <inheritdoc />
    public partial class Updatedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "RoomPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "RoomPlayers");
        }
    }
}
