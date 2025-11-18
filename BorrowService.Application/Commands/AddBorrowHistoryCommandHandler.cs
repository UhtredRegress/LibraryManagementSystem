using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure;
using BorrowService.Infrastructure.IRepository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using Shared;

namespace BorrowService.Application.Commands;

public class AddBorrowHistoryCommandHandler: IRequestHandler<AddBorrowHistoryCommand, Result<IEnumerable<BorrowHistory>>>
{
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IBorrowerRepository _borrowerRepository;
    private readonly IGrpcClient _grpcClient;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AddBorrowHistoryCommandHandler> _logger;

    public AddBorrowHistoryCommandHandler(IBorrowHistoryRepository borrowHistoryRepository,
        IBorrowerRepository borrowerRepository, IGrpcClient grpcClient, IEventBus eventBus,  ILogger<AddBorrowHistoryCommandHandler> logger)
    {
        _borrowHistoryRepository = borrowHistoryRepository;
        _borrowerRepository = borrowerRepository;
        _grpcClient = grpcClient;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<BorrowHistory>>> Handle(AddBorrowHistoryCommand request,
        CancellationToken cancellationToken)
    {

        if (!int.TryParse(request.userId, out int id))
        {
            return Result.Fail(new Error("Invalid user id"));
        }

        var foundBorrower = await _borrowerRepository.GetBorrowerByIdAsync(id) ??
                            await _borrowerRepository.CreateBorrowerAsync(new Borrower(id: id, name: request.Name,
                                phone: request.Phone, email: request.Email, address: request.Address));

        var historyBorrow =
            await _borrowHistoryRepository.GetBorrowHistoryFilteredAsync(bh =>
                bh.BorrowerId == foundBorrower.Id && bh.Status != BorrowStatus.Done && bh.ReturnDate == null);

        var requestHistory = historyBorrow.Where(hb => request.bookList.Contains(hb.BookId)).ToList();
        
        if (requestHistory.Count + request.bookList.Count() > 10)
        {
            return Result.Fail(new Error($"You can only borrow at most 10 books and you have alreay borrow {requestHistory.Count} books"));
        }
        
        var onLoanBooks = requestHistory.Where(hb => request.bookList.Contains(hb.BookId)).ToList();
        
        if (onLoanBooks.Count > 0)
        {
            return Result.Fail(new Error("There is already a borrow history for these book").WithMetadata("historyList", onLoanBooks));
        }
        
        var resultCheckBookRequest = await _grpcClient.RequestCheckExistedBook(request.bookList);

        if (resultCheckBookRequest == false)
        {
            return Result.Fail(new Error("Book not found in the library")); 
        }

        var borrowHistoryList = new List<BorrowHistory>(); 

        foreach (var bookId in request.bookList)
        {
            var borrowHistory = new BorrowHistory(borrowerId: id,bookId: bookId,days: request.Days);
            borrowHistoryList.Add(borrowHistory);
        }
        
        await _borrowHistoryRepository.CreateRangeBorrowHistoryAsync(borrowHistoryList);

        await _eventBus.PublishAsync(new BorrowHistoryCreatedIntegratedEvent(email: request.Email, bookIds: request.bookList,
                startDate: borrowHistoryList.First().StartDate, endDate: borrowHistoryList.First().EndDate, userName: request.Name));

        return Result.Ok<IEnumerable<BorrowHistory>>(borrowHistoryList);
    }
}