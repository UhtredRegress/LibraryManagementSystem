using BookService.Application.IntegrationEventHandler;
using RabbitMQEventBus;

namespace BookService.Presentation;

public class SubscribeHandlerService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly  ILogger<SubscribeHandlerService> _logger;

    public SubscribeHandlerService(IEventBus eventBus, ILogger<SubscribeHandlerService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting subscribe integration handler ");
        await _eventBus.SubscribeAsync<ConfirmBookReturnedIntegratedEvent, ConfirmBookReturnedIntegrationEventHandler>();
        await _eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, UpdateBookIntegrationHandler>();
    }
    
}