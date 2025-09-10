using LMS.BorrowService.Domain.Entity;
using LMS.BorrowService.Infrastructure;
using LMS.BorrowService.Infrastructure.IRepository;
using MediatR;

namespace LMS.BorrowService.Application.Commands;

public class AddBorrowHistoryCommandHandler: IRequestHandler<AddBorrowHistoryCommand, bool>
{
    private readonly IBorrowHistoryRepository _borrowHistoryRepository;
    private readonly IBorrowerRepository _borrowerRepository;
    private readonly IGrpcClient _grpcClient;

    public AddBorrowHistoryCommandHandler(IBorrowHistoryRepository borrowHistoryRepository,
        IBorrowerRepository borrowerRepository, IGrpcClient grpcClient)
    {
        _borrowHistoryRepository = borrowHistoryRepository;
        _borrowerRepository = borrowerRepository;
        _grpcClient = grpcClient;
    }
    
    public async Task<bool> Handle(AddBorrowHistoryCommand request, CancellationToken cancellationToken)
    {
        var foundBorrower = await _borrowerRepository.GetBorrowerByIdAsync(request.Id) ?? 
                            await _borrowerRepository.CreateBorrowerAsync(new Borrower(name: request.Name, phone: request.Phone, email: request.Email, address: request.Address));

        var resultCheckBoookRequest = await _grpcClient.RequestCheckExistedBook(request.bookList);
        return resultCheckBoookRequest;
    }
}