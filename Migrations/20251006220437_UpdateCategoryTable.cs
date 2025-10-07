using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_users_UsersUserId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_UsersUserId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UsersUserId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "categories",
                newName: "user_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "categories",
                type: "character varying(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_categories_user_id",
                table: "categories",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_users_user_id",
                table: "categories",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_users_user_id",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_user_id",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "categories",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddColumn<string>(
                name: "UsersUserId",
                table: "categories",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_UsersUserId",
                table: "categories",
                column: "UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_users_UsersUserId",
                table: "categories",
                column: "UsersUserId",
                principalTable: "users",
                principalColumn: "user_id");
        }
    }
}
