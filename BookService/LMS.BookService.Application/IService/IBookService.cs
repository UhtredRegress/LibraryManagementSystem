using LMS.BookService.Domain.Enum;
using LMS.BookService.Domain.Model;

namespace LMS.BookService.Application.IService;

public interface IBookService
{
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(int id, Book book);
    Task<Book> DeleteBookAsync(int id);
    Task<IEnumerable<Book>> GetBooksByPublishedDate(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Book>> GetBooksByAvailability(Availability availability);
    Task<IEnumerable<Book>> GetBooksByTitle(string title);
    Task<IEnumerable<Book>> UpdateRangeBooksAsync(IEnumerable<int> bookId);
}