using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.SmartLimit.Services;
using GS_Shop_UserManagement.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GS_Shop_UserManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISmartLimitationService, SmartLimitationService>();
        return services;
    }
}