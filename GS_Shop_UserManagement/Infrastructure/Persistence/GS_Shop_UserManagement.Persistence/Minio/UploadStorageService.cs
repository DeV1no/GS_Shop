using FileTypeChecker;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;

namespace GS_Shop_UserManagement.Persistence.Minio;

public class UploadStorageService<TEntity>(IMinioClient minioClient)
    : IUploadStorageService<TEntity>
    where TEntity : class
{
    public async Task<string> UploadFileAsync(IFormFile formFile,string? previousFilePath, 
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(previousFilePath))
            await DeleteFileAsync(previousFilePath);
            
        var entityName = GetEntityAsStringHelper.GetEntityAsStrings<TEntity>().ToLower();
        var isBucketExisted = await DoesBucketExistAsync(entityName);
        if (!isBucketExisted)
            await CreateBucketAsync(entityName, cancellationToken);
        var bas64ConvertedFile = ConvertIFormFileToBase64(formFile);
        await using var ms = new MemoryStream(Convert.FromBase64String(bas64ConvertedFile));
        var fileName = $"{Guid.NewGuid():N}.{FileTypeValidator.GetFileType(ms).Extension}";
        ms.Position = 0;
        var fileExtension = GetFileExtension(fileName);
        var createFileArgs = new PutObjectArgs()
            .WithBucket(entityName)
            .WithStreamData(ms)
            .WithObjectSize(ms.Length)
            .WithObject(fileName)
            .WithContentType(!string.IsNullOrEmpty(fileExtension)
                ? fileExtension
                : "application/octet-stream");

        var response = await minioClient.PutObjectAsync(createFileArgs, cancellationToken: cancellationToken);
        return entityName + "/" + response.ObjectName;
    }

   
    
    
    private static string GetFileExtension(string fileName)
    {
        var lastDotIndex = fileName.LastIndexOf('.');
        return fileName[(lastDotIndex + 1)..];
    }
   


    private static string ConvertIFormFileToBase64(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        memoryStream.Position = 0; // Reset position to beginning
        var bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }

    private async Task<bool> DoesBucketExistAsync(string bucketName)
    {
        try
        {
            return await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking bucket existence: {ex.Message}");
            return false;
        }
    }

    private async Task CreateBucketAsync(string name,
        CancellationToken cancellationToken = default)
    {
        var createBucketArgs = new MakeBucketArgs().WithBucket(name);
        await minioClient.MakeBucketAsync(createBucketArgs, cancellationToken);
    }
    private async Task<bool> DeleteFileAsync(string filePath)
    {
        var seperatedPath = BucketPathSeparator.Separat(filePath);
        try
        {
           
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(seperatedPath.Item1)
                .WithObject(seperatedPath.Item2);
            await minioClient.RemoveObjectAsync(removeObjectArgs);
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file: {ex.Message}");
            return false; 
        }
    }
}