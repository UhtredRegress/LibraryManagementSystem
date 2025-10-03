using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BorrowService.Application.Commands;

public record ConfirmBookReturnedCommand(int BorrowHistoryId, int UserId): IRequest<Result<BorrowHistory>>;

public class ConfirmBookReturnedCommandHandler : IRequestHandler<ConfirmBookReturnedCommand, Result<BorrowHistory>>
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

    public async Task<Result<BorrowHistory>> Handle(ConfirmBookReturnedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConfirmBookReturnedCommand received, starting process");
        BorrowHistory foundBorrowHistory = await _borrowHistoryRepository.GetBorrowHistoryByIdAsync(request.BorrowHistoryId);

        if (foundBorrowHistory == null)
        {
            return Result.Fail(new Error("Borrow History requested to confirm returned is not found"));
        }

        if (foundBorrowHistory.Status != BorrowStatus.Pending)
        {
            return Result.Fail(new Error("Borrow History requested is not returned by user"));
        }

        _logger.LogInformation("Found borrow history, now update status of borrow history");
        foundBorrowHistory.UpdateReturnInformation(request.UserId);
        
        _logger.LogInformation("Update borrow history to database");
        await _borrowHistoryRepository.UpdateBorrowHistoryAsync(foundBorrowHistory);
        
        _logger.LogInformation("Publish Confirm Book Returned Integration Event"); 
        await _eventBus.PublishAsync(new ConfirmBookReturnedIntegratedEvent(foundBorrowHistory.BookId));
            
        _logger.LogInformation("ConfirmBookReturnedCommand received, finishing process");
        
        return Result.Ok(foundBorrowHistory);
    }
}