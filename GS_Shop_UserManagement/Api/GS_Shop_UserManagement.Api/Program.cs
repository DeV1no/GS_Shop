using GS_Shop_UserManagement.Application;
using GS_Shop_UserManagement.Persistence;
using Microsoft.OpenApi.Models;
using GS_Shop_UserManagement.Infrastructure;
using GS_Shop_UserManagement.Infrastructure.Policy;

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
            Title = "HR Management Api"
        });
    });
}