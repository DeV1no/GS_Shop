using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GS_Shop_UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserClaimLimitationActionField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "UserLimitationClaims",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "UserLimitationClaims");
        }
    }
}
