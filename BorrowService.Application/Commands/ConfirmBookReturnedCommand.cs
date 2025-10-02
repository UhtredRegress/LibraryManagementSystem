using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BorrowService.Application.Commands;

public record ConfirmBookReturnedCommand(int BorrowHistoryId, int UserId): IRequest<BorrowHistory>;

public class ConfirmBookReturnedCommandHandler : IRequestHandler<ConfirmBookReturnedCommand, BorrowHistory>
{
    
    private readonly ILogger<ConfirmBookReturnedCommandHandler> _logger;
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IEventBus _eventBus;

    public ConfirmBookReturnedCommandHandler(ILogger<ConfirmBookReturnedCommandHandler> logger,  IBorrowHistoryRepository borrowHistoryRepository,  IEventBus eventBus)
    {
        _logger = logger;
        _borrowHistoryRepository = borrowHistoryRepository;
        _eventBus = eventBus;
    }

    public async Task<BorrowHistory> Handle(ConfirmBookReturnedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConfirmBookReturnedCommand received, starting process");
        BorrowHistory foundBorrowHistory = await _borrowHistoryRepository.GetBorrowHistoryByIdAsync(request.BorrowHistoryId);

        if (foundBorrowHistory == null || foundBorrowHistory.Status != BorrowStatus.Pending)
        {
            _logger.LogInformation("Borrow History not found, throwing exception");
            throw new InvalidDataException("There is no borrow history for this request");
        }

        _logger.LogInformation("Found borrow history, now update status of borrow history");
        foundBorrowHistory.UpdateReturnDate(request.UserId);
        
        _logger.LogInformation("Update borrow history to database");
        await _borrowHistoryRepository.UpdateBorrowHistoryAsync(foundBorrowHistory);
        
        _logger.LogInformation("Publish Confirm Book Returned Integration Event"); 
        await _eventBus.PublishAsync(new ConfirmBookReturnedIntegratedEvent(foundBorrowHistory.BookId));
        
        _logger.LogInformation("ConfirmBookReturnedCommand received, finishing process");
        
        return foundBorrowHistory;
    }
}