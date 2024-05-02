using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.Helpers;
using Minio;
using Minio.DataModel.Args;

namespace GS_Shop_UserManagement.Persistence.Minio;

public class DownloadStorageService(IMinioClient minioClient):IDownloadStorageService
{
    public async Task<string> GetObjectDownloadLink(
        string downloadLink,
        CancellationToken cancellationToken = default)
    {
        var separatedPath = BucketPathSeparator.Separat(downloadLink);
        var downloadLinkArgs = new PresignedGetObjectArgs()
            .WithBucket(separatedPath.Item1)
            .WithObject(separatedPath.Item2)
            .WithExpiry((int) TimeSpan.FromMinutes(10).TotalSeconds);
        var presignedUrl = await minioClient.PresignedGetObjectAsync(downloadLinkArgs);

        if (IsImageUrl(presignedUrl))
        {
            return presignedUrl;
        }

        var downloadLinkRequest = await minioClient.PresignedGetObjectAsync(downloadLinkArgs);
        return downloadLinkRequest;
    }

    private static string Bucket(string downloadLink, out string fileName)
    {
        var parts = downloadLink.Split('/');
        var bucket = parts[0];
        fileName = Path.GetFileName(downloadLink);
        return bucket;
    }

    private bool IsImageUrl(string url)
    {
        string[] imageExtensions = {".jpg", ".jpeg", ".png", ".gif"};
        return imageExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}