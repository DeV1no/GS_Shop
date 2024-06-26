﻿using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Persistence.FileManager.Services;
using GS_Shop_UserManagement.Persistence.Minio;
using GS_Shop_UserManagement.Persistence.Repositories;
using GS_Shop_UserManagement.Persistence.SmartLimit.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace GS_Shop_UserManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(ISmartLimitationService<>), typeof(SmartLimitationService<>));
        services.AddScoped(typeof(IFileService<>), typeof(FileService<>));
        services.AddMinio(client =>
        {
            client.WithCredentials("minio2","MinioDemo@123")
                .WithEndpoint("localhost:9000")
                .WithSSL(false);
        });
        services.AddScoped(typeof(IUploadStorageService<>), typeof(UploadStorageService<>));
        services.AddDbContext<GSShopUserManagementDbContext>(opt =>
        {
            opt.UseMySql(configuration.GetConnectionString("UserManagementConnectionString"),
                new MySqlServerVersion(new Version(8, 0, 23))); // Specify the MySQL server version
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
        services.AddScoped<IDownloadStorageService, DownloadStorageService>();
        return services;
    }
}