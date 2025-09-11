using System.Collections.Immutable;
using LMS.NotificationService;
using MailKit.Net.Smtp;
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
builder.Services.AddScoped<ISmtpClient, SmtpClient>();
builder.Services.AddScoped<IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>, BorrowHistoryNotificationHandler>();
builder.Services.AddScoped<IEmailService,SmtpEmailService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope()) {
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    await eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, BorrowHistoryNotificationHandler>("borrow_queue");
 }

host.Run();