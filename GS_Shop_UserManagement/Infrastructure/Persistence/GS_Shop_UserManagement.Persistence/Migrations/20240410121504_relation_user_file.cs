using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GS_Shop_UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class relation_user_file : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileDetailsId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FileDetailsId",
                table: "Users",
                column: "FileDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FileDetails_FileDetailsId",
                table: "Users",
                column: "FileDetailsId",
                principalTable: "FileDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FileDetails_FileDetailsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FileDetailsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FileDetailsId",
                table: "Users");
        }
    }
}
