
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GS_Shop_UserManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IMongoLoggerContext<>), typeof(MongoLoggerContext<>));
        services.AddScoped<IMongoLoggerService ,MongoLoggerService>();
        return services;
    }
}