using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;

namespace GS_Shop_UserManagement.Persistence.Minio;

public class DownloadStorageService(IMinioClient minioClient,IHttpContextAccessor httpContextAccessor):IDownloadStorageService
{
    public async Task<string> GetObjectDownloadLink(
        string downloadLink,
        CancellationToken cancellationToken = default)
    {
        var separatedPath = BucketPathSeparator.Separat(downloadLink);
        PermissionChecker(separatedPath);
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

    private void PermissionChecker(Tuple<string, string> separatedPath)
    {
        var claimChecker=  new UserClaimChecker(httpContextAccessor,"Get"+separatedPath.Item1);
        var check = claimChecker.Check();
        if (!check)
            throw new Exception("BZN b Chak");
    }

    private bool IsImageUrl(string url)
    {
        string[] imageExtensions = {".jpg", ".jpeg", ".png", ".gif"};
        return imageExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}