using StackExchange.Redis;
using IDatabase = Microsoft.EntityFrameworkCore.Storage.IDatabase;

namespace GS_Shop_UserManagement.Infrastructure.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly StackExchange.Redis.IDatabase _database;

    public RedisCacheService(ConnectionMultiplexer redisConnection)
    {
        _database = redisConnection.GetDatabase();
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _database.StringGetAsync(key);
    }

    public string Get(string key)
    {
        return  _database.StringGet(key);
    }

    public async Task SetAsync(string key, string value, TimeSpan expiry)
    {
        await _database.StringSetAsync(key, value, expiry);
    }
}