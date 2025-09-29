using System.Linq.Expressions;
using BorrowService.Domain.Entity;

namespace BorrowService.Infrastructure.IRepository;

public interface IBorrowHistoryRepository
{
    Task<BorrowHistory> GetBorrowHistoryByIdAsync(int id);
    Task<IEnumerable<BorrowHistory>> GetBorrowHistoryFilteredAsync(Expression<Func<BorrowHistory, bool>> predicate);
    Task<BorrowHistory> CreateBorrowHistoryAsync(BorrowHistory borrowHistory);
    Task<BorrowHistory> UpdateBorrowHistoryAsync(BorrowHistory borrowHistory);
    Task<BorrowHistory> DeleteBorrowHistoryAsync(BorrowHistory borrowHistory);
    Task<bool> CreateRangeBorrowHistoryAsync(IEnumerable<BorrowHistory> borrowHistory);
}