using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Application.DTOs.User;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IFormFile? ProfilePic { get; set; }
    public string? PreviousFilePath { get; set; }

}