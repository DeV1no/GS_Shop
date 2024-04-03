using MongoDB.Driver;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;

public interface IMongoLoggerContext<TEntity>
{
    IMongoCollection<TEntity> Entity { get; }
}