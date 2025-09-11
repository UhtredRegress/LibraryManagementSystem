using System.Text;
using LMS.BookService.Application;
using LMS.BookService.Application.IntegrationEvent;
using LMS.BookService.Application.IService;
using LMS.BookService.Infrastructure;
using LMS.BookService.Infrastructure.Interface;
using LMS.BookService.Presentation;
using LMS.BookService.Presentation.Authorization;
using LMS.BookService.Presentation.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using RabbitMQEventBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<BookServiceDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer <your-token>"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Add JWT Bearer
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NumericRolePolicy", policy =>
        policy.Requirements.Add(new NumericRoleRequirement(roleRequirement: 2)));
});

builder.Services.AddSingleton<IAuthorizationHandler, NumericRoleHandler>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5079); // HTTP
    options.ListenAnyIP(7080,
        listenOptions => listenOptions.Protocols = HttpProtocols.Http2); // gRPC or HTTPS if needed
});

builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddSingleton<IConnection>(sp =>
{
    var connectionFactory = new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
    return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IChannel>(sp =>
    sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult()
);

builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>,UpdateBookIntegrationHandler>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddSingleton<IConnection>(sp =>
{
    var connectionFactory = new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
    return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IChannel>(sp =>
    sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());


builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();

//Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookServiceDbContext>();
    db.Database.Migrate();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();
await eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, UpdateBookIntegrationHandler>("borrow_queue");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<BookAPIService>();

app.Run();