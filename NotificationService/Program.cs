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

var host = builder.Build();

using (var scope = host.Services.CreateScope()) {
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    await eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, BorrowHistoryNotificationHandler>();
    await eventBus.SubscribeAsync<ConfirmEmailIntegratedEvent, ConfirmEmailIntegratedEventHandler>();
}

host.Run();