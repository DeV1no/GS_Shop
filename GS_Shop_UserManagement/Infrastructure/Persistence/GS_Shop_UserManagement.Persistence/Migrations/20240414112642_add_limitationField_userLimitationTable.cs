using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GS_Shop_UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_limitationField_userLimitationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LimitationField",
                table: "UserLimitationClaims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitationField",
                table: "UserLimitationClaims");
        }
    }
}
