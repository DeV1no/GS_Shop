using System.Security.Claims;
using EventBus.Messages.Common;
using GS_Shop_UserManagement.Api.EventBusConsumer;
using GS_Shop_UserManagement.Persistence;
using GS_Shop_UserManagement.Infrastructure.CustomHealthCheck;
using GS_Shop_UserManagement.Infrastructure.Policy;
using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GS_Shop_UserManagement.Application;
using GS_Shop_UserManagement.Infrastructure;
using GS_Shop_UserManagement.Infrastructure.Auth;
using Microsoft.OpenApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
AddSwagger(builder.Services);
builder.Configuration
    .AddJsonFile("policyRequirements.json", optional: true, reloadOnChange: true)
    .AddJsonFile("limitationClaims.json", optional: true, reloadOnChange: true);
builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.ConfigureApplicationServices(builder.Configuration);
builder.Services.ConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<LoginConsumer>();
    cfg.AddConsumer<RegisterConsumer>();
    cfg.UsingRabbitMq((ctx, conf) =>
    {
        conf.Host("amqp://guest:guest@localhost:5672");
        conf.ReceiveEndpoint(EventBusConstants.LoginQueue, c => { c.ConfigureConsumer<LoginConsumer>(ctx); });
        conf.ReceiveEndpoint(EventBusConstants.RegisterQueue, c => { c.ConfigureConsumer<RegisterConsumer>(ctx); });
    });
});
builder.Services.AddMassTransitHostedService();
builder.Services.AddScoped<LoginConsumer>();
var policyRequirements = AuthorizationPolicyLoader.LoadPolicies(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    foreach (var policyName in policyRequirements.Keys)
    {
        options.AddPolicy(policyName, policy =>
        {
            var requiredClaims = policyRequirements[policyName];
            foreach (var claim in requiredClaims)
            {
                policy.RequireClaim(claim);
                policy.Requirements.Add(new RedisAuthorizationRequirement());
            }


            // Add RedisClaimsRequirement
            // Or initialize it with appropriate permissions


            // Combine requirements into a single requirement
        });
    }
});

var policyConfiguration = PolicyConfigurationReader.ReadPolicyConfiguration("./policyRequirements.json");

// Add authorization policies


builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>(nameof(ApiHealthCheck))
    .AddDbContextCheck<GSShopUserManagementDbContext>();
builder.Services.AddHealthChecksUI(opt => { opt.AddHealthCheckEndpoint("HealthCheck Api", "/api/HealthCheckStatus"); })
    .AddInMemoryStorage();
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = builder.Configuration
        .GetValue<string>("CacheSettings:ConnectionString")!;
    options.Configuration = $"{redisConnectionString},defaultDatabase=1";
});

//services.AddSingleton<ConnectionMultiplexer>(provider =>
//{
//    var configuration = ConfigurationOptions.Parse(Configuration["Redis:ConnectionString"]);
//   return ConnectionMultiplexer.Connect(configuration);
//});
builder.Services.AddSingleton<IAuthorizationHandler, RedisAuthorizationHandler>();

var app = builder.Build();

app.UseHangfireDashboard().UseHangfireServer();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.MapHealthChecksUI(opt => opt.UIPath = "/dashboard");
app.MapGet("/api/HealthCheckStatus", () =>
{
    var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
    return healthCheckService.CheckHealthAsync();
});

app.Run();
return;

void AddSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(o =>
    {
        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Description = "Jwt Authentication",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        o.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });

        o.SwaggerDoc("v1", new OpenApiInfo()
        {
            Version = "v1",
            Title = "User Api"
        });
    });
}

public class PolicyConfiguration
{
    public List<PolicyItem> AuthorizationPolicies { get; set; }
}

public class PolicyItem
{
    public string PolicyName { get; set; }
    public List<string> RequiredClaims { get; set; }
}

public static class PolicyConfigurationReader
{
    public static PolicyConfiguration ReadPolicyConfiguration(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<PolicyConfiguration>(json);
    }
}