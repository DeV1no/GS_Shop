using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GS_Shop_UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class user_chage_file : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FileDetails_FileDetailsId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "FileDetailsId",
                table: "Users",
                newName: "ProfilePictureId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_FileDetailsId",
                table: "Users",
                newName: "IX_Users_ProfilePictureId");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FileDetails_ProfilePictureId",
                table: "Users",
                column: "ProfilePictureId",
                principalTable: "FileDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FileDetails_ProfilePictureId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureId",
                table: "Users",
                newName: "FileDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ProfilePictureId",
                table: "Users",
                newName: "IX_Users_FileDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FileDetails_FileDetailsId",
                table: "Users",
                column: "FileDetailsId",
                principalTable: "FileDetails",
                principalColumn: "Id");
        }
    }
}
