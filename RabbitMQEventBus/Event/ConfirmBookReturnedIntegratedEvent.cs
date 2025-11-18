namespace RabbitMQEventBus;

public class ConfirmBookReturnedIntegratedEvent : IntegrationEvent
{
    public int BookId { get; }
    
    public ConfirmBookReturnedIntegratedEvent(int bookId)
    {
        BookId = bookId;
    }
}