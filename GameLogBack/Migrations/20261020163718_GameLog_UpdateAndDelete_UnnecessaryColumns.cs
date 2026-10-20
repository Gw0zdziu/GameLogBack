using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class GameLog_UpdateAndDelete_UnnecessaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_confirm_confirm_users",
                table: "confirm_confirm_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code_recovery_password",
                table: "code_recovery_password");

            migrationBuilder.DropColumn(
                name: "refresh_token_id",
                table: "refresh_token_info");

            migrationBuilder.DropColumn(
                name: "confirm_code_id",
                table: "confirm_confirm_users");

            migrationBuilder.DropColumn(
                name: "recovery_code_id",
                table: "code_recovery_password");

            migrationBuilder.RenameColumn(
                name: "confirm_code",
                table: "confirm_confirm_users",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "recovery_code",
                table: "code_recovery_password",
                newName: "code");

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token",
                table: "refresh_token_info",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "confirm_confirm_users",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "code_recovery_password",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_confirm_confirm_users",
                table: "confirm_confirm_users",
                column: "code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_code_recovery_password",
                table: "code_recovery_password",
                column: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_confirm_confirm_users",
                table: "confirm_confirm_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code_recovery_password",
                table: "code_recovery_password");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "confirm_confirm_users",
                newName: "confirm_code");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "code_recovery_password",
                newName: "recovery_code");

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token",
                table: "refresh_token_info",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

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
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "confirm_code_id",
                table: "confirm_confirm_users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "recovery_code",
                table: "code_recovery_password",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "recovery_code_id",
                table: "code_recovery_password",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_confirm_confirm_users",
                table: "confirm_confirm_users",
                column: "confirm_code_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_code_recovery_password",
                table: "code_recovery_password",
                column: "recovery_code_id");
        }
    }
}
