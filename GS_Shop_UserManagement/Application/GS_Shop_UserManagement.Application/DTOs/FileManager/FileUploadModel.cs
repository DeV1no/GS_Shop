using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Application.DTOs.FileManager;

public class FileUploadModel
{
    public IFormFile FileDetails { get; set; }
}