using System.Text;
using FluentValidation;
using LMS.Bussiness.Service;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQEventBus;
using UserService.Application;
using UserService.Infrastructure;
using UserService.Infrastructure.Interface;
using UserService.Infrastructure.Service;
using UserService.Infrastructure.Service.Interface;
using UserService.Presentation.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(@"D:\Code\Project\LibraryManagementSystem\dev-secrets\appsettings.shared.json", optional: true);


// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Needed for minimal APIs too
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
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BookService.Application.Business.AssemblyMarker).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(BookService.Application.Business.AssemblyMarker));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<ITokenService, TokenService>();

// Configure Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{   
    options.Configuration = "localhost:6379"; // or your Redis server connection
    options.InstanceName = "EmailConfirm_";   // prefix for keys
});

builder.Services.AddScoped<IEmailTokenService, EmailTokenService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // allow all origins
            .AllowAnyMethod()   // allow all HTTP methods
            .AllowAnyHeader();  // allow all headers
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization(options => options.AddPolicy("AdminRoleRequirement", policy =>
{
    policy.AddRequirements(new NumericRoleRequirement(4));
    policy.RequireClaim("status", "Active");
}));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5078); 
});

builder.Services.AddSingleton<IAuthorizationHandler, NumericRoleHandler>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
app.MapOpenApi();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
