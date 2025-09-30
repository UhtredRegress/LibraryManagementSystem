using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQEventBus;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQEventBus( IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task CreateConnection()
    {
        var connectionFactory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
        };
        _connection = await connectionFactory.CreateConnectionAsync();
    }

    public async Task CreateChannel()
    {
        if (_connection is null)
        {
            await CreateConnection();
        }

        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        if (_connection is null)
        {
            await CreateConnection();
        }
        using var _channel = await _connection.CreateChannelAsync();
        var eventName = @event.GetType().Name;
        await _channel.ExchangeDeclareAsync(eventName, ExchangeType.Fanout, durable: true);
        var message = JsonSerializer.Serialize((object)@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(message);
        
        await _channel.BasicPublishAsync(
            exchange: eventName,
            routingKey: "",
            mandatory: false,
            body: body);
    }
    

    public async Task SubscribeAsync<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        if (_channel is null)
        {
            await CreateChannel();
        }
        var queueName = typeof(TH).Name;
        var eventName = typeof(T).Name;
        await _channel.ExchangeDeclareAsync(eventName, ExchangeType.Fanout, durable: true);
        
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        
        await _channel.QueueBindAsync(queue: queueName, exchange: eventName, routingKey: eventName);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var @event = JsonSerializer.Deserialize<T>(message);

            using var scope = _serviceScopeFactory.CreateScope();
            var handlerList = scope.ServiceProvider.GetServices<IIntegrationEventHandler<T>>();
            foreach (var handler in handlerList)
            {
                await handler.Handle(@event);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };
        
        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }
}