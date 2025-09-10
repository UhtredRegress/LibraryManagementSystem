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
    
    public async Task<BorrowHistory> GetBorrowerByIdAsync(int id)
    {
        return await _context.BorrowHistories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<BorrowHistory>> GetBorrowersFilteredAsync(Expression<Func<BorrowHistory, bool>> predicate)
    {
        return await _context.BorrowHistories.Where(predicate).ToListAsync();
    }

    public async Task<BorrowHistory> CreateBorrowerAsync(BorrowHistory borrowHistory)
    {
        await _context.BorrowHistories.AddAsync(borrowHistory);
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<BorrowHistory> UpdateBorrowerAsync(BorrowHistory borrowHistory)
    {
        _context.BorrowHistories.Update(borrowHistory);
        await _context.SaveChangesAsync();
        return borrowHistory;
    }

    public async Task<BorrowHistory> DeleteBorrowerAsync(BorrowHistory borrowHistory)
    {
        _context.BorrowHistories.Remove(borrowHistory); 
        await _context.SaveChangesAsync();
        return borrowHistory;
    }
}