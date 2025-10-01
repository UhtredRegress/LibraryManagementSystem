using System.Linq.Expressions;
using BorrowService.Domain.Entity;
using BorrowService.Domain.ValueObject;
using BorrowService.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BorrowService.Infrastructure;

public class BorrowHistoryRepository : IBorrowHistoryRepository
{
    private readonly BorrowDbContext _context;

    public BorrowHistoryRepository(BorrowDbContext context)
    {
        _context = context;
    }
    
    public async Task<BorrowHistory> GetBorrowHistoryByIdAsync(int id)
    {
        return await _context.BorrowHistories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<BorrowHistory>> GetBorrowHistoryFilteredAsync(Expression<Func<BorrowHistory, bool>> predicate)
    {
        return await _context.BorrowHistories.Where(predicate).ToListAsync();
    }

    public async Task<BorrowHistory> CreateBorrowHistoryAsync(BorrowHistory borrowHistory)
    {
        await _context.BorrowHistories.AddAsync(borrowHistory);
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<BorrowHistory> UpdateBorrowHistoryAsync(BorrowHistory borrowHistory)
    {
        _context.BorrowHistories.Update(borrowHistory);
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<BorrowHistory> DeleteBorrowHistoryAsync(BorrowHistory borrowHistory)
    {
        _context.BorrowHistories.Remove(borrowHistory); 
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<bool> CreateRangeBorrowHistoryAsync(IEnumerable<BorrowHistory> borrowHistory)
    {
        await _context.BorrowHistories.AddRangeAsync(borrowHistory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BorrowHistory>> UpdateRangeBorrowHistoryAsync(
        IEnumerable<BorrowHistory> borrowHistory)
    {
        _context.UpdateRange(borrowHistory);
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<IEnumerable<BorrowHistory>> GetBorrowHistoryForReturnBookAsync(IEnumerable<int> bookList, int userId)
    {
        return await _context.BorrowHistories
            .Where(x => bookList.Contains(x.BookId))
            .Where(x => x.Status == BorrowStatus.Approved)
            .Where(x => x.BorrowerId == userId)
            .ToListAsync();
    }
    
}