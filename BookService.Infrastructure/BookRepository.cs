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
        return await _context.Books.Include(b => b.Authors).Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == id);
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

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorIds, int start, int number)
    {
        return await _context.Books.AsNoTracking()
            .Where(book => book.Authors.Any(author => author.Id == authorIds))
            .Include(book => book.BookCategories).ThenInclude(bookCategory => bookCategory.Category)
            .Include(book => book.Authors)
            .OrderBy(book => book.Id)
            .Skip(start)
            .Take(number)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksAsync(int page, int pageSize, int? type, ICollection<int>? authorsId,
        ICollection<int>? categoriesId,
        int? yearPublishedStart, int? yearPublishedEnd)
    {
        var query = _context.Books.AsNoTracking();
        if (type != null)
        {
            query = query.Where(book => ((int)book.Type & type) != 0);
        }

        if (authorsId != null)
        {
            query = query.Where(x => x.Authors.Any(a => authorsId.Contains(a.Id)));
        }

        if (categoriesId != null)
        {
            query = query.Where(x => x.Authors.Any(a => categoriesId.Contains(a.Id)));
        }

        if (yearPublishedStart != null && yearPublishedEnd != null)
        {
            query = query.Where(x =>
                x.PublishDate.Value.Year >= yearPublishedStart && x.PublishDate.Value.Year <= yearPublishedEnd);
        }

        return await query.Include(book => book.BookCategories).ThenInclude(cate => cate.Category)
            .Include(book => book.Authors)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<IEnumerable<Book>> UpdateRangeBookAsync(IEnumerable<Book> books)
    {
        _context.Books.UpdateRange(books);
        await _context.SaveChangesAsync();
        return books;
    }
}