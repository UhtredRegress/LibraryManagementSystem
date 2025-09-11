using System.Collections.Immutable;
using LMS.NotificationService;
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

builder.Services.AddSingleton<IChannel>(sp => sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>();


var host = builder.Build();
host.Run();