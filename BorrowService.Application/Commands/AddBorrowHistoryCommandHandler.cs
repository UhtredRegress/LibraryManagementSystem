using BorrowService.Domain.Entity;
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
    
    public async Task<Result<IEnumerable<BorrowHistory>>> Handle(AddBorrowHistoryCommand request, CancellationToken cancellationToken)
    {

        if (!int.TryParse(request.userId, out int id))
        {
            return Result.Fail(new Error("Invalid user id"));
        }
        var foundBorrower = await _borrowerRepository.GetBorrowerByIdAsync(id) ?? 
                            await _borrowerRepository.CreateBorrowerAsync(new Borrower(id: id, name: request.Name, phone: request.Phone, email: request.Email, address: request.Address));
        
        var historyBorrow = await _borrowHistoryRepository.GetBorrowHistoryFilteredAsync(book => book.Id == foundBorrower.Id && request.bookList.Contains(book.Id));

        if (historyBorrow.Count() != 0)
        {
            return Result.Fail(new Error("There is already a borrow history for this book").WithMetadata("historyList", historyBorrow));
        }
        
        var resultCheckBookRequest = await _grpcClient.RequestCheckExistedBook(request.bookList);

        if (resultCheckBookRequest == false)
        {
            return Result.Fail(new Error("Book not found")); 
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