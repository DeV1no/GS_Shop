using GS_Shop_UserManagement.Persistence.Minio.ServiceModels;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Persistence.Minio.Interfaces;

public interface IStorageService
{
    Task CreateBucketAsync(CreateBucketServiceModel createBucketModel, CancellationToken cancellationToken = default);
 
    Task<UploadFileServiceModelResult> UploadBase64FileAsync(UploadFileServiceModel uploadFileServiceModel,
        CancellationToken cancellationToken = default);

    Task<GetObjectDownloadLinkServiceModel> GetObjectDownloadLink(GetObjectDownloadLinkRequestModel requestModel,
        CancellationToken cancellationToken = default);

    public Task<UploadFileServiceModelResult> UploadFileAsync(string path,
        IFormFile file, CancellationToken cancellationToken = default);
}