using ApiGateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddSingleton<IMetricService, MetricService>();
builder.Services.AddHostedService<FlushWorker>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

var app = builder.Build();

// Enable Swagger UI for Ocelot
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<MetricCollectorMiddleware>();
await app.UseOcelot();

app.Run();