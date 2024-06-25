using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GS_Shop_UserManagement.Application.Models;

namespace GS_Shop_UserManagement.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add AutoMapper configuration from the assembly containing this code
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Add MediatR configuration from the assembly containing this code
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // jwt configs 
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
                        ClockSkew = TimeSpan.Zero, // No clock skew
                        
                    };
                   // options.TokenHandlers;
                });

            return services;

        }
    }
}