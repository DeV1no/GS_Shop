using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Persistence.FileManager.Models;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Persistence.FileManager.Services;

public interface IFileService
{
    public Task<string> PostFileAsync(IFormFile fileData);

    public Task PostMultiFileAsync(List<FileUploadModel> fileData);

    public Task<string> DownloadFileById(int fileName);
}