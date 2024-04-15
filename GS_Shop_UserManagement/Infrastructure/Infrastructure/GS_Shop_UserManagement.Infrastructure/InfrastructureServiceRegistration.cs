using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IMongoLoggerContext<>), typeof(MongoLoggerContext<>));
        services.AddScoped<IMongoLoggerService, MongoLoggerService>();

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
