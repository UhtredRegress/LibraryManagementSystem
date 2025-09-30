using BorrowService.Infrastructure.IRepository;
using MediatR;
using Shared.Exception;

namespace BorrowService.Application.Commands;

public record ReturnBookCommand(string UserId, string Email, string Name, string Address, string Phone, IEnumerable<int> BookList) : IRequest<bool>;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, bool>
{
    private readonly IBorrowerRepository _borrowerRepository;

    public ReturnBookCommandHandler(IBorrowerRepository borrowerRepository)
    {
        _borrowerRepository = borrowerRepository;
    }
    
    public async Task<bool> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
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
    }
}