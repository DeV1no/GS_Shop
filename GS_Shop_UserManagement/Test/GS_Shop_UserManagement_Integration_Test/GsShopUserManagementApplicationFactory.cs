using GS_Shop_UserManagement.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;


namespace GS_Shop_UserManagement_Integration_Test
{
    internal class GsShopUserManagementApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<GSShopUserManagementDbContext>));
                var connectionString = GetConnectionString();
                services.AddDbContext<GSShopUserManagementDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GSShopUserManagementDbContext>();
                    try
                    {
                        dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GsShopUserManagementApplicationFactory>>();
                        logger.LogError(ex, "An error occurred while initializing the test database.");
                        throw;
                    }
                }
            });
        }

        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GsShopUserManagementApplicationFactory>()
                .Build();
            var connString =
                "Server=localhost,1433;Database=GS_Shop_UserManagement_test;User Id=sa;Password=Admin123;TrustServerCertificate=True;";
            return connString;
        }
    }
}
