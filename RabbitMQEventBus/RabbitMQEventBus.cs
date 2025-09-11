using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQEventBus;

public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchangeName = "event_bus";
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQEventBus(IConnection connection, IChannel channel, IServiceScopeFactory serviceScopeFactory)
    {
        _connection = connection;
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        using var _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true);
        var eventName = @event.GetType().Name;
        var message = JsonSerializer.Serialize((object)@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(message);
        
        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventName,
            mandatory: false,
            body: body);
    }
    

    public async Task SubscribeAsync<T, TH>(string queueName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true);
        var eventName = typeof(T).Name;
        
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        
        await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: eventName);
        
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
}