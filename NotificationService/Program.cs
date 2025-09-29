using Lirabry.Grpc;
using MailKit.Net.Smtp;
using NotificationService;
using RabbitMQ.Client;
using RabbitMQEventBus;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(sp =>
{
    var connectionFactory = new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
    return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton(sp => sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());
builder.Services.AddSingleton<IChannel>(sp => sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();
builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>, BorrowHistoryNotificationHandler>();
builder.Services.AddScoped<IEmailService,MailKitEmailService>();
builder.Services.AddGrpcClient<BookAPI.BookAPIClient>(options =>
{
    options.Address = new Uri("http://localhost:7080");
});

var host = builder.Build();

using (var scope = host.Services.CreateScope()) {
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    await eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, BorrowHistoryNotificationHandler>();
}

host.Run();