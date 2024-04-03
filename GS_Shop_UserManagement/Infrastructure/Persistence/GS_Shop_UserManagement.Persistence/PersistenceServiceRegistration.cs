using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;
using GS_Shop_UserManagement.Persistence.Repositories;
using GS_Shop_UserManagement.Persistence.SmartLimit.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GS_Shop_UserManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(ISmartLimitationService<>), typeof(SmartLimitationService<>));

        services.AddDbContext<GSShopUserManagementDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("UserManagementConnectionString"));
        });

        services.AddIdentity<User, Role>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<GSShopUserManagementDbContext>()
            .AddDefaultTokenProviders();

        return services;

    }
}