using NotificationService.EventHandler;
using RabbitMQEventBus;

namespace NotificationService;

public class SubscribeHandlerService:IHostedService
{
    private readonly ILogger<SubscribeHandlerService> _logger;
    private readonly IEventBus _eventBus;

    public SubscribeHandlerService(ILogger<SubscribeHandlerService> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubscribeHandlerService is starting.");
        await _eventBus.SubscribeAsync<BorrowHistoryCreatedIntegratedEvent, BorrowHistoryNotificationHandler>();
        await _eventBus.SubscribeAsync<ConfirmEmailIntegratedEvent, ConfirmEmailIntegratedEventHandler>();
        await _eventBus.SubscribeAsync<NewBookCreatedIntegratedEvent, NewBookAddedEventHandler>();
        _logger.LogInformation("SubscribeHandlerService finished successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubscribeHandlerService is stopping.");
        return Task.CompletedTask;
    }
}