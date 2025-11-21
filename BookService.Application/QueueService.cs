using RabbitMQEventBus;

namespace BookService.Application;

public static class QueueService
{
    public static Queue<NewBookCreatedIntegratedEvent> Queue { get; } = new Queue<NewBookCreatedIntegratedEvent>();
}