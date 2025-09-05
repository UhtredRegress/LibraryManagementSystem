using System.Linq.Expressions;
using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id); 
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return book;
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
        return await _context.Books.Where(expression).ToListAsync();
    }
}