using BookService.Domain.Enum;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BookService.Application.IntegrationEventHandler;

public class ConfirmBookReturnedIntegrationEventHandler:IIntegrationEventHandler<ConfirmBookReturnedIntegratedEvent>
{
    private readonly ILogger<ConfirmBookReturnedIntegrationEventHandler> _logger;
    private readonly IBookRepository _bookRepository;

    public ConfirmBookReturnedIntegrationEventHandler(ILogger<ConfirmBookReturnedIntegrationEventHandler> logger,
        IBookRepository bookRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
    }
    public async Task Handle(ConfirmBookReturnedIntegratedEvent @event)
    {
        _logger.LogInformation("ConfirmBookReturnedCommand received, starting process"); 
        Book? foundBook = await _bookRepository.GetBookByIdAsync(@event.BookId);
        if (foundBook == null)
        {
            return;
        }
        foundBook.BookReturned();
        await _bookRepository.UpdateBookAsync(foundBook);
        _logger.LogInformation("ConfirmBookReturnedCommand received, finishing process");
    }
}