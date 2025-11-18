using System.Linq.Expressions;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public class BookPriceRepository : IBookPriceRepository
{
    private readonly BookServiceDbContext _context;

    public BookPriceRepository(BookServiceDbContext context)
    {
        _context = context;
    }
    public async Task<BookPrice?> GetBookPriceAsync(int bookId)
    {
        return await _context.BookPrices.FirstOrDefaultAsync(bp => bp.BookId == bookId);
    }

    public async Task<BookPrice> AddBookPriceAsync(BookPrice bookPrice)
    {
        var result = await _context.BookPrices.AddAsync(bookPrice);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<BookPrice> UpdateBookPriceAsync(BookPrice bookPrice)
    {
        _context.BookPrices.Update(bookPrice);
        await _context.SaveChangesAsync();
        return bookPrice;
    }

    public async Task<BookPrice> DeleteBookPriceAsync(BookPrice bookPrice)
    {
        _context.BookPrices.Remove(bookPrice);
        await _context.SaveChangesAsync();
        return bookPrice;
    }

    public async Task<BookPrice?> GetBookPriceFiltered(Expression<Func<BookPrice, bool>> predicate)
    {
        return await _context.BookPrices.FirstOrDefaultAsync(predicate);
    }
}