namespace GS_Shop_UserManagement.Application.Contracts.Persistence;

public interface IDownloadStorageService
{
    Task<string> GetObjectDownloadLink(string downloadLink,
        CancellationToken cancellationToken = default);
}