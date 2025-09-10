using System.Linq.Expressions;
using LMS.BorrowService.Domain.Entity;

namespace LMS.BorrowService.Infrastructure.IRepository;

public interface IBorrowHistoryRepository
{
    Task<BorrowHistory> GetBorrowerByIdAsync(int id);
    Task<IEnumerable<BorrowHistory>> GetBorrowersFilteredAsync(Expression<Func<BorrowHistory, bool>> predicate);
    Task<BorrowHistory> CreateBorrowerAsync(BorrowHistory borrowHistory);
    Task<BorrowHistory> UpdateBorrowerAsync(BorrowHistory borrowHistory);
    Task<BorrowHistory> DeleteBorrowerAsync(BorrowHistory borrowHistory);
}