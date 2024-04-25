
using System.Text;
using GS_Shop.Home.Services.DTOs.User;
using GS_Shop.Home.Services.IServices;
using GS_Shop.Home.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GS_Shop.Home.Services;

public static class ServiceRegistration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Configure Identity : is in PersistenceServiceRegistration

        // Configure JWT authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No clock skew
                };
            });

        return services;
    }
}