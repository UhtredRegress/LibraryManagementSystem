using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure;

public class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _context;

    public LoanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> AddLoanAsync(Loan loan)
    {
        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan> UpdateLoanAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<IEnumerable<Loan>> GetLoanAsyncByUserID(int userID)
    {
        return await _context.Loans.Where(u => u.UserId == userID).ToListAsync();
    }
    
}