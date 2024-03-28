using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GS_Shop_UserManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<GSShopUserManagementDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("UserManagementConnectionString"));
        });
        return services;
    }
}