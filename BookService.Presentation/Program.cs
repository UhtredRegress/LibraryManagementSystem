using System.Text;
using BookService.Application.IService;
using BookService.Infrastructure;
using BookService.Infrastructure.Interface;
using BookService.Presentation.Authorization;
using BookService.Presentation.Grpc;
using BookService.Application.IntegrationEventHandler;
using BookService.Presentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQEventBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService.Application.BookService>();
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
builder.Configuration
    .AddJsonFile(@"D:\Code\Project\LibraryManagementSystem\dev-secrets\appsettings.shared.json", optional: true);
    
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
    options.AddPolicy("StudentNumericRolePolicy", policy =>
        policy.Requirements.Add(new NumericRoleRequirement(roleRequirement: 2)));
});

builder.Services.AddSingleton<IAuthorizationHandler, NumericRoleHandler>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5079); // HTTP
    options.ListenAnyIP(7080,
        listenOptions => listenOptions.Protocols = HttpProtocols.Http2); // gRPC or HTTPS if needed
});


builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>,UpdateBookIntegrationHandler>();
builder.Services
    .AddScoped<IIntegrationEventHandler<RequestReturnBookIntegratedEvent>, RequestReturnBookIntegrationEventHandler>();
builder.Services
    .AddScoped<IIntegrationEventHandler<ConfirmBookReturnedIntegratedEvent>,
        ConfirmBookReturnedIntegrationEventHandler>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddAuthorization(options => options.AddPolicy("LibrarianNumericRolePolicy", policy =>
{
    policy.Requirements.Add(new NumericRoleRequirement(roleRequirement: 2));
}));

builder.Services.AddGrpc();
builder.Services.AddHostedService<SubscribeHandlerService>();

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

//var eventBus = app.Services.GetRequiredService<IEventBus>();
//await eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, UpdateBookIntegrationHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<BookAPIService>();

app.Run();