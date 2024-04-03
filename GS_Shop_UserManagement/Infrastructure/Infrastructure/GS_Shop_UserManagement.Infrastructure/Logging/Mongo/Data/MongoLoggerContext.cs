using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SharpCompress.Common;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;

public class MongoLoggerContext<TEntity> : IMongoLoggerContext<TEntity>
{

    public MongoLoggerContext(IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection("MongoSettings");
        var connectionString = databaseSettings["ConnectionString"];
        var databaseName = databaseSettings["DatabaseName"];
        var collectionName = databaseSettings["CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        Entity = database.GetCollection<TEntity>(collectionName);
    }
    public IMongoCollection<TEntity> Entity { get; }
}