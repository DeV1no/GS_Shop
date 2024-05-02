using FileTypeChecker;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GS_Shop_UserManagement.Persistence.Minio
{
    public class UploadStorageService<TEntity> : IUploadStorageService<TEntity>
        where TEntity : class
    {
        private readonly IMinioClient _minioClient;

        public UploadStorageService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<string> UploadFileAsync(IFormFile formFile, string? previousFilePath,
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(previousFilePath))
                await FileOperations.DeleteFileAsync(_minioClient, previousFilePath);

            var entityName = GetEntityAsStringHelper.GetEntityAsStrings<TEntity>().ToLower();
            var isBucketExisted = await MinioClientOperations.DoesBucketExistAsync(_minioClient, entityName);
            if (!isBucketExisted)
                await MinioClientOperations.CreateBucketAsync(_minioClient, entityName, cancellationToken);

            var base64ConvertedFile = FileOperations.ConvertIFormFileToBase64(formFile);
            await using var ms = new MemoryStream(Convert.FromBase64String(base64ConvertedFile));
            var fileName = $"{Guid.NewGuid():N}.{FileTypeValidator.GetFileType(ms).Extension}";
            ms.Position = 0;
            var fileExtension = FileOperations.GetFileExtension(fileName);
            var createFileArgs = new PutObjectArgs()
                .WithBucket(entityName)
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithObject(fileName)
                .WithContentType(!string.IsNullOrEmpty(fileExtension)
                    ? fileExtension
                    : "application/octet-stream");

            var response = await _minioClient.PutObjectAsync(createFileArgs, cancellationToken: cancellationToken);
            return entityName + "/" + response.ObjectName;
        }

        

        
    } 
     static class FileOperations
    {
        public static string GetFileExtension(string fileName)
        {
            var lastDotIndex = fileName.LastIndexOf('.');
            return fileName[(lastDotIndex + 1)..];
        }

        public static string ConvertIFormFileToBase64(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            memoryStream.Position = 0; // Reset position to beginning
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public static async Task<bool> DeleteFileAsync(IMinioClient minioClient, string filePath)
        {
            var separatedPath = BucketPathSeparator.Separat(filePath);
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(separatedPath.Item1)
                    .WithObject(separatedPath.Item2);
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
    static class MinioClientOperations
    {
        public static async Task<bool> DoesBucketExistAsync(IMinioClient minioClient, string bucketName)
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

        public static async Task CreateBucketAsync(IMinioClient minioClient, string name, CancellationToken cancellationToken = default)
        {
            var createBucketArgs = new MakeBucketArgs().WithBucket(name);
            await minioClient.MakeBucketAsync(createBucketArgs, cancellationToken);
        }
    }
}
