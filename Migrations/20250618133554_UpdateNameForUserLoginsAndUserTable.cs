using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameForUserLoginsAndUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropColumn(
                name: "user_email",
                table: "user_logins");

            migrationBuilder.RenameColumn(
                name: "lastname",
                table: "user_logins",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "firstname",
                table: "user_logins",
                newName: "password");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    firstname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    lastname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user_email = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_user_logins_users_user_id",
                table: "user_logins",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_logins_users_user_id",
                table: "user_logins");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "user_logins",
                newName: "lastname");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "user_logins",
                newName: "firstname");

            migrationBuilder.AddColumn<string>(
                name: "user_email",
                table: "user_logins",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_UserLogins_user_logins_user_id",
                        column: x => x.user_id,
                        principalTable: "user_logins",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
