using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationInRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_info_users_refresh_token_id",
                table: "refresh_token_info");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_info_user_id",
                table: "refresh_token_info",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_info_users_user_id",
                table: "refresh_token_info",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_info_users_user_id",
                table: "refresh_token_info");

            migrationBuilder.DropIndex(
                name: "IX_refresh_token_info_user_id",
                table: "refresh_token_info");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_info_users_refresh_token_id",
                table: "refresh_token_info",
                column: "refresh_token_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
