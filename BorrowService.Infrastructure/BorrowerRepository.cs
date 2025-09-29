using System.Linq.Expressions;
using BorrowService.Domain.Entity;
using BorrowService.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BorrowService.Infrastructure;

public class BorrowerRepository : IBorrowerRepository
{
    private readonly BorrowDbContext _context;

    public BorrowerRepository(BorrowDbContext context)
    {
        _context = context;
    }
    
    public async Task<Borrower> GetBorrowerByIdAsync(int id)
    {
        return await _context.Borrowers.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Borrower>> GetBorrowersFilteredAsync(Expression<Func<Borrower, bool>> predicate)
    {
        return await _context.Borrowers.Where(predicate).ToListAsync();
    }

    public async Task<Borrower> CreateBorrowerAsync(Borrower borrower)
    {
        await _context.Borrowers.AddAsync(borrower);
        await _context.SaveChangesAsync();
        return borrower;
    }

    public async Task<Borrower> UpdateBorrowerAsync(Borrower borrower)
    {
         _context.Borrowers.Update(borrower);
        await _context.SaveChangesAsync();
        return borrower;
    }

    public async Task<Borrower> DeleteBorrowerAsync(Borrower borrower)
    {
        _context.Borrowers.Remove(borrower);
        await _context.SaveChangesAsync();
        return borrower;
    }
}