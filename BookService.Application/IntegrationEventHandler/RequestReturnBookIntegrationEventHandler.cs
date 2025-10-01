using BookService.Domain.Enum;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BookService.Application.IntegrationEventHandler;

public class RequestReturnBookIntegrationEventHandler : IIntegrationEventHandler<RequestReturnBookIntegratedEvent>
{
    private readonly ILogger<RequestReturnBookIntegrationEventHandler> _logger;
    private readonly IBookRepository _bookRepository;

    public RequestReturnBookIntegrationEventHandler(ILogger<RequestReturnBookIntegrationEventHandler> logger,
        IBookRepository bookRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
    }
    public async Task Handle(RequestReturnBookIntegratedEvent @event)
    {
        _logger.LogInformation("Received RequestReturnBookIntegrationEvent, starting to process");
        IEnumerable<Book> bookList = await _bookRepository.GetRangeBookByIdAsync(@event.BookList);
        foreach (var book in bookList)
        {
            book.UpdateAvailability(Availability.Pending);
        }

        await _bookRepository.UpdateRangeBookAsync(bookList);
        _logger.LogInformation("Received RequestReturnBookIntegrationEvent, process finished");
    }
}