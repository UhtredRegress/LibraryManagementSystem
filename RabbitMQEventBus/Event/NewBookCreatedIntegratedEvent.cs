namespace RabbitMQEventBus;

public class NewBookCreatedIntegratedEvent : IntegrationEvent
{
    public string Title {get; set;}
    public string Author {get; set;}
    public string Category { get; set; }
}