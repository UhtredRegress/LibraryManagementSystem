using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using MediatR;
using RabbitMQEventBus;
using Shared.Exception;

namespace BorrowService.Application.Commands;

public record ReturnBookCommand(string UserId, string Email, string Name, string Address, string Phone, IEnumerable<int> BookList) : IRequest<IEnumerable<BorrowHistory>>;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, IEnumerable<BorrowHistory>>
{
    private readonly IBorrowerRepository _borrowerRepository;
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IEventBus _eventBus;

    public ReturnBookCommandHandler(IBorrowerRepository borrowerRepository, IBorrowHistoryRepository borrowHistoryRepository, IEventBus eventBus)
    {
        _borrowerRepository = borrowerRepository;
        _borrowHistoryRepository = borrowHistoryRepository;
        _eventBus = eventBus;
    }
    
    public async Task<IEnumerable<BorrowHistory>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(request.UserId, out int id))
        {
            throw new InvalidDataException("UserId is not valid");
        }

        var foundUser = await _borrowerRepository.GetBorrowerByIdAsync(id);

        if (foundUser == null)
        {
            throw new NotFoundDataException("User is not found in the system");
        }

        var activeLoan = await _borrowHistoryRepository.GetBorrowHistoryForReturnBookAsync(bookList: request.BookList, userId: foundUser.Id);

        if (activeLoan.Count() == 0)
        {
            throw new NotFoundDataException("There are no borrow history for this request");
        }
        
        foreach (BorrowHistory historyLoanItem in activeLoan)
        {
            historyLoanItem.UpdateStatus(BorrowStatus.Pending);
        }
        
        var returnBookList = await _borrowHistoryRepository.UpdateRangeBorrowHistoryAsync(activeLoan);
        var resultBookId = activeLoan.Select(al => al.BookId).ToList();
        
        await _eventBus.PublishAsync(new RequestReturnBookIntegratedEvent(username: foundUser.Name, phone: foundUser.Phone, email: foundUser.Email, bookList: resultBookId));
        return returnBookList;
    }
}