using BookService.Application.IService;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BookService.Application.IntegrationEvent;

public class UpdateBookIntegrationHandler : IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent> 
{
    private readonly IBookService _bookService;
    private readonly ILogger<UpdateBookIntegrationHandler> _logger;

    public UpdateBookIntegrationHandler(IBookService bookService, ILogger<UpdateBookIntegrationHandler> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }
    
    public async Task Handle(BorrowHistoryCreatedIntegratedEvent @event)
    {
        _logger.LogInformation("Received UpdateBookIntegrationEvent");

        await _bookService.UpdateRangeBooksAsync(@event.BookIds);
        
        _logger.LogInformation("UpdateBookIntegrationHandler Finished"); 
        
    }
}