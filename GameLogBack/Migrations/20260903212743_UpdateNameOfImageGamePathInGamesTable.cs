using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameOfImageGamePathInGamesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GameImagePath",
                table: "games",
                newName: "game_image_path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "game_image_path",
                table: "games",
                newName: "GameImagePath");
        }
    }
}
