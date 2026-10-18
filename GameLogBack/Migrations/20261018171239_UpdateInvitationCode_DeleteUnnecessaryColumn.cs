using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvitationCode_DeleteUnnecessaryColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitation_codes",
                table: "invitation_codes");

            migrationBuilder.DropColumn(
                name: "invitation_code_id",
                table: "invitation_codes");

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
                name: "invitation_code",
                table: "invitation_codes",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitation_codes",
                table: "invitation_codes",
                column: "invitation_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitation_codes",
                table: "invitation_codes");

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
                name: "invitation_code",
                table: "invitation_codes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "invitation_code_id",
                table: "invitation_codes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token_info",
                table: "refresh_token_info",
                column: "refresh_token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitation_codes",
                table: "invitation_codes",
                column: "invitation_code_id");
        }
    }
}
