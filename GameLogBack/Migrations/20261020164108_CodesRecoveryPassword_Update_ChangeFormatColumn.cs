using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLogBack.Migrations
{
    /// <inheritdoc />
    public partial class CodesRecoveryPassword_Update_ChangeFormatColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "code_recovery_password",
                newName: "is_used");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_used",
                table: "code_recovery_password",
                newName: "IsUsed");
        }
    }
}
