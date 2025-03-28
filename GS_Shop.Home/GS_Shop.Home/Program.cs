using GS_Shop_UserManagement.Infrastructure.Policy;
using GS_Shop.Home.Services;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:5107") // Update with your Blazor frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
// Add services to the container.
builder.Configuration
    .AddJsonFile("policyRequirements.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
AddSwagger(builder.Services);
builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((ctx, conf) =>
    {
        // conf.Host(builder.Configuration.GetValue<string>("EventBussSettings:HostAddress"));
        conf.Host("amqp://guest:guest@localhost:5672");
    });
});
builder.Services.AddStackExchangeRedisCache(opt =>
    opt.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString")
);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddMassTransitHostedService();
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
            }
        });
    }
});
var app = builder.Build();

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

// app cors 
app.UseCors("AllowBlazorClient");
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
            Title = "Home Api"
        });
    });
}
