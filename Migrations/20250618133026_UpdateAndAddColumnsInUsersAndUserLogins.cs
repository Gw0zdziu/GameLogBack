using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAndAddColumnsInUsersAndUserLogins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins");

            migrationBuilder.DropIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Users",
                newName: "user_email");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "lastname");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "UserLogins",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserLogins",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserLoginId",
                table: "UserLogins",
                newName: "username");

            migrationBuilder.AlterColumn<string>(
                name: "user_email",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "firstname",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_user_id",
                table: "UserLogins",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_user_id",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "firstname",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "user_email",
                table: "Users",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "lastname",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "UserLogins",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserLogins",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "UserLogins",
                newName: "UserLoginId");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins",
                column: "UserLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
