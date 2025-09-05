using System.Linq.Expressions;
using LMS.Domain.Model;

namespace LMS.Infrastructure.Interface;

public interface IBookRepository
{
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task<Book> DeleteBook(Book book);
    Task<IEnumerable<Book>> GetBooksFilteredAsync(Expression<Func<Book, bool>> expression);
}