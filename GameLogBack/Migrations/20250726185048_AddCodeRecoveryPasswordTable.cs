using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeRecoveryPasswordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "confirm_code_users");

            migrationBuilder.CreateTable(
                name: "code_recovery_password",
                columns: table => new
                {
                    recovery_code_id = table.Column<string>(type: "text", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<string>(type: "character varying(100)", nullable: false),
                    recovery_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_recovery_password", x => x.recovery_code_id);
                    table.ForeignKey(
                        name: "FK_code_recovery_password_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "confirm_confirm_users",
                columns: table => new
                {
                    confirm_code_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "character varying(100)", nullable: false),
                    confirm_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirm_confirm_users", x => x.confirm_code_id);
                    table.ForeignKey(
                        name: "FK_confirm_confirm_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_code_recovery_password_user_id",
                table: "code_recovery_password",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_confirm_confirm_users_user_id",
                table: "confirm_confirm_users",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "code_recovery_password");

            migrationBuilder.DropTable(
                name: "confirm_confirm_users");

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
    }
}
