using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "refresh_token_info",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "refresh_token_info");
        }
    }
}
