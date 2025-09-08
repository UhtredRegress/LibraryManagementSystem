using LMS.Domain.Model;

namespace LMS.Infrastructure.Interface;

public interface ILoanRepository
{
    Task<Loan> AddLoanAsync(Loan loan);
    Task<Loan> UpdateLoanAsync(Loan loan);
    Task<IEnumerable<Loan>> GetLoanAsyncByUserID(int userID);
}