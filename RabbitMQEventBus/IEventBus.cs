namespace RabbitMQEventBus;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
    Task SubscribeAsync<T, TH>(string queueName) where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;
}