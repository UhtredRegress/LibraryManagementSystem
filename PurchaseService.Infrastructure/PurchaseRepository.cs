using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PurchaseService.Domain.Aggregate;

namespace PurchaseService.Infrastructure;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly PurchaseDbContext _context;

    public PurchaseRepository(PurchaseDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseBook> AddPurchaseBookAsync(PurchaseBook purchaseBook)
    {
        var result = await _context.PurchaseBooks.AddAsync(purchaseBook);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<PurchaseBook> UpdatePurchaseBookAsync(PurchaseBook purchaseBook)
    {
        _context.PurchaseBooks.Update(purchaseBook);
        await _context.SaveChangesAsync();
        return purchaseBook;
    }

    public async Task<PurchaseBook?> GetPurchaseBookAsync(int id)
    {
        return await _context.PurchaseBooks.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PurchaseBook> DeletePurchaseBookAsync(PurchaseBook purchaseBook)
    {
        _context.PurchaseBooks.Remove(purchaseBook);
        await _context.SaveChangesAsync();
        return purchaseBook;
    }

    public async Task<IEnumerable<PurchaseBook>> GetRangePurchaseBooksFilterAsync(
        Expression<Func<PurchaseBook, bool>> predicate)
    {
        return await _context.PurchaseBooks.Where(predicate).ToListAsync();
    }

    public async Task<PurchaseBook?> GetPurchaseBookFilteredAsync(Expression<Func<PurchaseBook, bool>> predicate)
    {
        return await _context.PurchaseBooks.FirstOrDefaultAsync(predicate);
    }
}