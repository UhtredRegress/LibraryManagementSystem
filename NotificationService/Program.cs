using Lirabry.Grpc;
using MailKit.Net.Smtp;
using NotificationService;
using RabbitMQ.Client;
using RabbitMQEventBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>, BorrowHistoryNotificationHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<ConfirmEmailIntegratedEvent>, ConfirmEmailIntegratedEventHandler>();
builder.Services.AddScoped<IEmailService,MailKitEmailService>();
builder.Services.AddGrpcClient<BookAPI.BookAPIClient>(options =>
{
    options.Address = new Uri("http://localhost:7080");
});
builder.Services.AddHostedService<SubscribeHandlerService>();

var host = builder.Build();

host.Run();