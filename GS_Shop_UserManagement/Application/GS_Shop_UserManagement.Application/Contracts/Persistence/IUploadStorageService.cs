using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Application.Contracts.Persistence;

public interface IUploadStorageService<TEntity> where TEntity : class
{
    Task<string> UploadFileAsync(IFormFile formFile,string? previousFilePath,
        CancellationToken cancellationToken = default);
}
