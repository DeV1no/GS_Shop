using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Persistence.FileManager.Models;

public class FileUploadModel
{


    public IFormFile FileDetails { get; set; }
}