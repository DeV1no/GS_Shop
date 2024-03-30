namespace GS_Shop_UserManagement.Infrastructure.SmartLimit.Services;

public interface ISmartLimitationService
{
    Task<IEnumerable<int>> GetLimitedIdsAsync(int userId, string limitationClaim);

}