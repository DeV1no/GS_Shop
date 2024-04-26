using GS_Shop.Home.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
