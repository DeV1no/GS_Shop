using GS_Shop_UserManagement.Persistence.Minio.Interfaces;
using GS_Shop_UserManagement.Persistence.Minio.ServiceModels;
using Minio;
using Minio.DataModel.Args;
using FileTypeChecker;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Persistence.Minio.Implementations;

public class StorageService : IStorageService
{
    private readonly IMinioClient _minioClient;

    public StorageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task CreateBucketAsync(CreateBucketServiceModel createBucketModel,
        CancellationToken cancellationToken = default)
    {
        var createBucketArgs = new MakeBucketArgs().WithBucket(createBucketModel.Name);

        await _minioClient.MakeBucketAsync(createBucketArgs, cancellationToken);
    }

    public async Task<UploadFileServiceModelResult> UploadBase64FileAsync(UploadFileServiceModel uploadFileServiceModel,
        CancellationToken cancellationToken = default)
    {
        var bas64ConvertedFile = ConvertIFormFileToBase64(uploadFileServiceModel.FileContent);
        await using var ms = new MemoryStream(Convert.FromBase64String(bas64ConvertedFile));

        var fileName = $"{Guid.NewGuid():N}.{FileTypeValidator.GetFileType(ms).Extension}";

        ms.Position = 0;

        var createFileArgs = new PutObjectArgs()
            .WithBucket(uploadFileServiceModel.Bucket)
            .WithStreamData(ms)
            .WithObjectSize(ms.Length)
            .WithObject(fileName)
            .WithContentType(!string.IsNullOrEmpty(uploadFileServiceModel.FileType)
                ? uploadFileServiceModel.FileType
                : "application/octet-stream");

        var response = await _minioClient.PutObjectAsync(createFileArgs, cancellationToken: cancellationToken);

        return new UploadFileServiceModelResult(response.ObjectName, uploadFileServiceModel.Bucket);
    }

    public async Task<GetObjectDownloadLinkServiceModel> GetObjectDownloadLink(
        GetObjectDownloadLinkRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        var downloadLinkArgs = new PresignedGetObjectArgs()
            .WithBucket(requestModel.BucketName)
            .WithObject(requestModel.FileName)
            .WithExpiry((int)TimeSpan.FromMinutes(10).TotalSeconds);


        var downloadLinkRequest = await _minioClient.PresignedGetObjectAsync(downloadLinkArgs);

        return new GetObjectDownloadLinkServiceModel(downloadLinkRequest);
    }

    public async Task<UploadFileServiceModelResult> UploadFileAsync(string name,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;

        var fileName = $"{Guid.NewGuid():N}.{FileTypeValidator.GetFileType(ms).Extension}";

        var createFileArgs = new PutObjectArgs()
            .WithBucket(name)
            .WithStreamData(ms)
            .WithObjectSize(ms.Length)
            .WithObject(fileName)
            .WithContentType(file.ContentType);

        var response = await _minioClient.PutObjectAsync(createFileArgs, cancellationToken);

        return new UploadFileServiceModelResult(response.ObjectName, name);
    }
    
    private static string ConvertIFormFileToBase64(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        memoryStream.Position = 0; // Reset position to beginning
        var bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }
}