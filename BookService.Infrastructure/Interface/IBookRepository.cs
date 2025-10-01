using System.Linq.Expressions;
using BookService.Domain.Model;


namespace BookService.Infrastructure.Interface;

public interface IBookRepository
{
    Task<IEnumerable<Book>> UpdateRangeBookAsync(IEnumerable<Book> books);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task<Book> DeleteBook(Book book);
    Task<IEnumerable<Book>> GetBooksFilteredAsync(Expression<Func<Book, bool>> expression);
    Task<IEnumerable<Book>> GetRangeBookByIdAsync(IEnumerable<int> bookIds);
}