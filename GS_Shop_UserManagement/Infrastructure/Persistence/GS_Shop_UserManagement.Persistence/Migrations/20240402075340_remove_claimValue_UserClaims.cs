using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GS_Shop_UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class remove_claimValue_UserClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimValue",
                table: "UserClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimValue",
                table: "UserClaims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
