namespace GS_Shop_UserManagement.Infrastructure.Redis;

public interface IRedisCacheService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, TimeSpan expiry);
}