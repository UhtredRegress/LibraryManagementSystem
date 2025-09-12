using LMS.BorrowService.Application.Validator;
using LMS.BorrowService.Infrastructure.IRepository;
using MediatR;

namespace LMS.BorrowService.Application.Commands;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, bool>
{
    private readonly IBorrowerRepository _borrowerRepository;
    
    public Task<bool> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        
    }
}