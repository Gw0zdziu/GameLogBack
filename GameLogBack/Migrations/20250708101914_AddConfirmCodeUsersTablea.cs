using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmCodeUsersTablea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "confirm_code_users",
                columns: table => new
                {
                    confirm_code_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "character varying(100)", nullable: false),
                    confirm_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirm_code_users", x => x.confirm_code_id);
                    table.ForeignKey(
                        name: "FK_confirm_code_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_confirm_code_users_user_id",
                table: "confirm_code_users",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "confirm_code_users");
        }
    }
}
