using System.Text;
using Lirabry.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotificationService;
using NotificationService.DbContext;
using NotificationService.EventHandler;
using NotificationService.Infrastructure.Hub;
using NotificationService.Metric;
using NotificationService.Service;
using NotificationService.Service.Interface;
using OpenTelemetry.Metrics;
using RabbitMQEventBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<NotiDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>, BorrowHistoryNotificationHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<ConfirmEmailIntegratedEvent>, ConfirmEmailIntegratedEventHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<NewBookCreatedIntegratedEvent>, NewBookAddedEventHandler>(); 
builder.Services.AddScoped<IEmailService,MailKitEmailService>();
builder.Services.AddSignalR();
builder.Services.AddGrpcClient<BookAPI.BookAPIClient>(options =>
{
    options.Address = new Uri("http://localhost:7080");
});

builder.Services.AddHostedService<SubscribeHandlerService>();

//Add Jwt Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; 
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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Receive event on JWTBearerEvents {accessToken}", context.Request.Query["access_token"]);
                // Allow token from query string for SignalR
                var accessToken = context.Request.Query["access_token"];

                // If the request is for the hub endpoint:
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/book-noti"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // Your Vite dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                  // This is REQUIRED for SignalR + JWT
    });
});
// Add Authorization
builder.Services.AddAuthorization();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });
    

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddScoped<ISubscribeNotiService, SubscribeNotiService>();
builder.Services.AddSingleton<WebSocketMetric>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddPrometheusExporter();
        builder.AddMeter("LMS.WS");
        builder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel");
    });

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();
app.UseCors("ReactApp");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotiBookHub>("/book-noti");
app.Run();