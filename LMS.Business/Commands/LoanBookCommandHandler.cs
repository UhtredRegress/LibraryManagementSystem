using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using MediatR;

namespace LMS.Business.Commands;

public record LoanBookCommand(int userId, int bookId) : IRequest<bool>;

public class LoanBookCommandHandler : IRequestHandler<LoanBookCommand, bool> 
{
    private readonly ILoanRepository _loanRepository;

    public LoanBookCommandHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<bool> Handle(LoanBookCommand request, CancellationToken cancellationToken)
    {
        var newLoan = new Loan();
        
        newLoan.BookId = request.bookId;
        newLoan.UserId = request.userId;
        newLoan.LoanDate = DateTime.UtcNow;

        try
        {
            _loanRepository.AddLoanAsync(newLoan);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}