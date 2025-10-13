using BookService.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace BookService.Application.IService;

public interface IBookService
{
    Task<Book> AddBookAsync(BookAddDTO bookAddDTO);
    Task<Book> UpdateBookAsync(int id, Book book);
    Task<Book> DeleteBookAsync(int id);
    Task<IEnumerable<Book>> GetBooksByPublishedDate(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Book>> GetBooksByTitle(string title);
    Task<IEnumerable<Book>> UpdateRangeBooksAsync(IEnumerable<int> bookId);
    Task<Book> UpdateFileForBookId(int id, IFormFile file);
}