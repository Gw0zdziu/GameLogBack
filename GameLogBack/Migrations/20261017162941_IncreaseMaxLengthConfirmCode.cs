using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseMaxLengthConfirmCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token",
                table: "refresh_token_info",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token_id",
                table: "refresh_token_info",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "confirm_code",
                table: "confirm_confirm_users",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.DropColumn(
                name: "refresh_token_id",
                table: "refresh_token_info");

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token",
                table: "refresh_token_info",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "confirm_code",
                table: "confirm_confirm_users",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token");
        }
    }
}
