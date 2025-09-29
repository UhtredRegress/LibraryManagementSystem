using BorrowService.Domain.Entity;
using BorrowService.Infrastructure;
using BorrowService.Infrastructure.IRepository;
using MediatR;
using RabbitMQEventBus;

namespace BorrowService.Application.Commands;

public class AddBorrowHistoryCommandHandler: IRequestHandler<AddBorrowHistoryCommand, bool>
{
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IBorrowerRepository _borrowerRepository;
    private readonly IGrpcClient _grpcClient;
    private readonly IEventBus _eventBus;

    public AddBorrowHistoryCommandHandler(IBorrowHistoryRepository borrowHistoryRepository,
        IBorrowerRepository borrowerRepository, IGrpcClient grpcClient, IEventBus eventBus)
    {
        _borrowHistoryRepository = borrowHistoryRepository;
        _borrowerRepository = borrowerRepository;
        _grpcClient = grpcClient;
        _eventBus = eventBus;
    }
    
    public async Task<bool> Handle(AddBorrowHistoryCommand request, CancellationToken cancellationToken)
    {
        var foundBorrower = await _borrowerRepository.GetBorrowerByIdAsync(request.Id) ?? 
                            await _borrowerRepository.CreateBorrowerAsync(new Borrower(id: request.Id, name: request.Name, phone: request.Phone, email: request.Email, address: request.Address));

        var resultCheckBoookRequest = await _grpcClient.RequestCheckExistedBook(request.bookList);

        if (resultCheckBoookRequest == false)
        {
            return false; 
        }

        ICollection<BorrowHistory> borrowHistoryList = new List<BorrowHistory>(); 

        foreach (var bookId in request.bookList)
        {
            var borrowHistory = new BorrowHistory(borrowerId: request.Id,bookId: bookId,days: request.Days);
            borrowHistoryList.Add(borrowHistory);
        }
        
        var result = await _borrowHistoryRepository.CreateRangeBorrowHistoryAsync(borrowHistoryList);

        if (result == true)
        {
            await _eventBus.PublishAsync(new BorrowHistoryCreatedIntegratedEvent(email: request.Email, bookIds: request.bookList,
                startDate: borrowHistoryList.First().StartDate, endDate: borrowHistoryList.First().EndDate, userName: request.Name));
        }
        else
        {
            return false;
        }

        return true; 
    }
}