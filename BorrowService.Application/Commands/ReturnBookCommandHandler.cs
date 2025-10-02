using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using Shared.Exception;

namespace BorrowService.Application.Commands;

public record ReturnBookCommand(string UserId, string Email, string Name, string Address, string Phone, IEnumerable<int> BookList) : IRequest<IEnumerable<BorrowHistory>>;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, IEnumerable<BorrowHistory>>
{
    private readonly IBorrowerRepository _borrowerRepository;
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReturnBookCommandHandler> _logger;

    public ReturnBookCommandHandler(IBorrowerRepository borrowerRepository, IBorrowHistoryRepository borrowHistoryRepository, IEventBus eventBus, ILogger<ReturnBookCommandHandler> logger)
    {
        _borrowerRepository = borrowerRepository;
        _borrowHistoryRepository = borrowHistoryRepository;
        _eventBus = eventBus;
        _logger = logger;
    }
    
    public async Task<IEnumerable<BorrowHistory>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ReturnBookCommand received, starting process");
        
        _logger.LogInformation("Validate request user id");
        if (!int.TryParse(request.UserId, out int id))
        {
            _logger.LogInformation("User id is not valid, throwing exception");
            throw new InvalidDataException("UserId is not valid");
        }


        _logger.LogInformation("Retrieve borrower by id in database");
        var foundUser = await _borrowerRepository.GetBorrowerByIdAsync(id);

        if (foundUser == null)
        {
            _logger.LogInformation("Borrower not found, throwing exception");
            throw new NotFoundDataException("User is not found in the system");
        }

        _logger.LogInformation("Retrieve borrow histories made by user in database");
        var activeLoan = await _borrowHistoryRepository.GetBorrowHistoryForReturnBookAsync(bookList: request.BookList, userId: foundUser.Id);

        if (activeLoan.Count() == 0)
        {
            _logger.LogInformation("No active borrow history found, throwing exception");
            throw new NotFoundDataException("There are no borrow history for this request");
        }

        _logger.LogInformation("Update status of borrow history");
        foreach (BorrowHistory historyLoanItem in activeLoan)
        {
            historyLoanItem.UpdateStatus(BorrowStatus.Pending);
        }

        _logger.LogInformation("Update data in database");
        var returnBookList = await _borrowHistoryRepository.UpdateRangeBorrowHistoryAsync(activeLoan);
        var resultBookId = activeLoan.Select(al => al.BookId).ToList();

        _logger.LogInformation("Publish integration event RequestReturnBookIntegratedEvent");
        await _eventBus.PublishAsync(new RequestReturnBookIntegratedEvent(username: foundUser.Name, phone: foundUser.Phone, email: foundUser.Email, bookList: resultBookId));
        return returnBookList;
    }
}