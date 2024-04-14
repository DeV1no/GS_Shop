using GS_Shop_UserManagement.Application.DTOs.FileManager;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Application.Contracts.Persistence;

public interface IFileService<TPath> where TPath : class
{
    public Task<Tuple<int, string>> PostFileAsync(IFormFile fileData, int? previousFileId=null);

    public Task PostMultiFileAsync(List<FileUploadModel> fileData);

    public Task<string> DownloadFileById(int fileName);
}
