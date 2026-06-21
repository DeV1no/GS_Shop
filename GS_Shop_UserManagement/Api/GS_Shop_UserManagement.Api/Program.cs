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
using GS_Shop_UserManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GS_Shop_UserManagement.Application.Models;
using Microsoft.OpenApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
    cfg.AddConsumer<GetUserConsumer>();
    cfg.AddConsumer<GetUserPublicConsumer>();
    cfg.UsingRabbitMq((ctx, conf) =>
    {
        var rabbitUri = builder.Configuration.GetValue<string>("EventBusSettings:HostAddress");
        if (string.IsNullOrEmpty(rabbitUri) || rabbitUri.Contains("localhost"))
        {
             // Fallback or skip
        }
        conf.Host(rabbitUri ?? "amqp://guest:guest@localhost:5672");
        conf.ReceiveEndpoint(EventBusConstants.LoginQueue, c => { c.ConfigureConsumer<LoginConsumer>(ctx); });
        conf.ReceiveEndpoint(EventBusConstants.UserListQueue, c => { c.ConfigureConsumer<GetUserConsumer>(ctx); });
        conf.ReceiveEndpoint(EventBusConstants.RegisterQueue, c => { c.ConfigureConsumer<RegisterConsumer>(ctx); });
        conf.ReceiveEndpoint(EventBusConstants.UserListPublicQueue, c => { c.ConfigureConsumer<GetUserPublicConsumer>(ctx); });
    });
});
builder.Services.AddMassTransitHostedService();
builder.Services.AddScoped<LoginConsumer>();

builder.Services.AddHttpContextAccessor(); // Ensure IHttpContextAccessor is registered

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
                policy.Requirements.Add(new RedisAuthorizationRequirement());
            }
        });
    }
}); 

var policyConfiguration = PolicyConfigurationReader.ReadPolicyConfiguration("./policyRequirements.json");

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });


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

builder.Services.AddSingleton<IAuthorizationHandler, RedisAuthorizationHandler>();



var app = builder.Build();



// Set the ServiceProvider
ServiceLocator.ServiceProvider = app.Services;

if (!app.Environment.EnvironmentName.Contains("Testing"))
{
    var hangfireConnectionString = builder.Configuration.GetValue<string>("HangfireSettings:ConnectionString");
    if (!string.IsNullOrEmpty(hangfireConnectionString) && hangfireConnectionString != "mongodb://localhost:27017")
    {
        app.UseHangfireDashboard().UseHangfireServer();
    }
}

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

app.MapGet("/api/HealthCheckStatus", async context =>
{
    var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
    var report = await healthCheckService.CheckHealthAsync();
    var result = JsonConvert.SerializeObject(report);

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(result);
});

app.Run();

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
