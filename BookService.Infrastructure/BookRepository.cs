using System.Linq.Expressions;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public class BookRepository : IBookRepository
{
    private readonly BookServiceDbContext _context;

    public BookRepository(BookServiceDbContext context)
    {
        _context = context;
    }
    public async Task<Book> GetBookByIdAsync(int id)
    {
        return await _context.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id); 
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        var result = await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> DeleteBook(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return book;
    }   

    public async Task<IEnumerable<Book>> GetBooksFilteredAsync(Expression<Func<Book, bool>> expression)
    {
        return await _context.Books
            .Where(expression).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetRangeBookByIdAsync(IEnumerable<int> bookIds)
    {
        return await _context.Books.Where(book => bookIds.Contains(book.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Book>> UpdateRangeBookAsync(IEnumerable<Book> books)
    {
        _context.Books.UpdateRange(books);
        await _context.SaveChangesAsync();
        return books;
    }
}