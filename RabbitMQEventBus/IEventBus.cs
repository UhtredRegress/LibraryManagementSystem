namespace RabbitMQEventBus;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
    Task SubscribeAsync<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;
}