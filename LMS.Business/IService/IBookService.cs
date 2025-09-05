using LMS.Domain.Enum;
using LMS.Domain.Model;

namespace LMS.Business.IService;

public interface IBookService
{
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(int id, Book book);
    Task<Book> DeleteBookAsync(int id);
    Task<IEnumerable<Book>> GetBooksByPublishedDate(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Book>> GetBooksByAvailability(Availability availability);
    Task<IEnumerable<Book>> GetBooksByTitle(string title);
}