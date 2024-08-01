using Microsoft.Extensions.DependencyInjection;
namespace GS_Shop_UserManagement.Infrastructure.Helpers;

public static class ServiceLocator
{
    public static IServiceProvider? ServiceProvider { get; set; }
}
