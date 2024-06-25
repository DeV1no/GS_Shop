using GS_Shop_UserManagement.Infrastructure.Auth;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;
using GS_Shop_UserManagement.Infrastructure.Redis;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

namespace GS_Shop_UserManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IMongoLoggerContext<>), typeof(MongoLoggerContext<>));
        services.AddScoped<IMongoLoggerService, MongoLoggerService>();
        services.AddSingleton<IRedisCacheService, RedisCacheService>();


        services.AddSingleton<ConnectionMultiplexer>(provider =>
        {
            var configurations = configuration.GetValue<string>("CacheSettings:ConnectionString");
            var multiplexer = ConnectionMultiplexer.Connect(configurations!);

            // Assuming the desired database number is 1, replace it with your desired database number.
            multiplexer.GetDatabase(1);
            return multiplexer;
        });


        // Retrieve Hangfire settings from appsettings.json
        var hangfireConnectionString = configuration.GetValue<string>("HangfireSettings:ConnectionString");
        var hangfireDatabaseName = configuration.GetValue<string>("HangfireSettings:DatabaseName");

        // Configure Hangfire
        var mongoClient = new MongoClient(hangfireConnectionString);
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMongoStorage(
                mongoClient,
                hangfireDatabaseName,
                new MongoStorageOptions
                {
                    Prefix = "hangfire:",
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,

                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new DropMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    }
                }));

        // Add Hangfire server
        services.AddHangfireServer();

        return services;
    }
}