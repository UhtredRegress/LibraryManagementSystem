using System.Linq.Expressions;
using LMS.BorrowService.Domain.Entity;
using LMS.BorrowService.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace LMS.BorrowService.Infrastructure;

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
}