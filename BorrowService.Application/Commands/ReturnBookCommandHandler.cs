using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;


namespace BorrowService.Application.Commands;

public record ReturnBookCommand(string UserId, string Email, string Name, string Address, string Phone, IEnumerable<int> BookList) : IRequest<Result<IEnumerable<BorrowHistory>>>;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, Result<IEnumerable<BorrowHistory>>>
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
    
    public async Task<Result<IEnumerable<BorrowHistory>>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ReturnBookCommand received, starting process");
        
        _logger.LogInformation("Validate request user id");
        if (!int.TryParse(request.UserId, out int id))
        {
            return Result.Fail(new Error("Invalid user id").WithMetadata("Id", request.UserId));
        }
        
        _logger.LogInformation("Retrieve borrower by id in database");
        var foundUser = await _borrowerRepository.GetBorrowerByIdAsync(id);

        if (foundUser == null)
        {
            _logger.LogInformation("Borrower not found, throwing exception");
           return Result.Fail(new Error("Borrower not found").WithMetadata("Id", id));
        }

        _logger.LogInformation("Retrieve borrow histories made by user in database");
        var activeLoan =
            await _borrowHistoryRepository.GetBorrowHistoryFilteredAsync(bh =>
                bh.BorrowerId == foundUser.Id && request.BookList.Contains(bh.BookId) &&
                bh.Status == BorrowStatus.Approved && bh.ReturnDate == null);

        int activeLoanCount = activeLoan.Count();
        int bookListCount = request.BookList.Count();

        if (activeLoanCount < bookListCount)
        {
            return Result.Fail(new Error("Your request some books that is not valid to returned"));
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
        return Result.Ok(returnBookList);
    }
}