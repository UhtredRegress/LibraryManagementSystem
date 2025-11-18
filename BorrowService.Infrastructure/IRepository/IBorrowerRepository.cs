using System.Linq.Expressions;
using BorrowService.Domain.Entity;

namespace BorrowService.Infrastructure.IRepository;

public interface IBorrowerRepository
{
    Task<Borrower> GetBorrowerByIdAsync(int id);
    Task<IEnumerable<Borrower>> GetBorrowersFilteredAsync(Expression<Func<Borrower, bool>> predicate);
    Task<Borrower> CreateBorrowerAsync(Borrower borrower);
    Task<Borrower> UpdateBorrowerAsync(Borrower borrower);
    Task<Borrower> DeleteBorrowerAsync(Borrower borrower);
}