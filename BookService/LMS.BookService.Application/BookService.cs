using LMS.BookService.Domain.Enum;
using LMS.BookService.Domain.Model;
using LMS.BookService.Domain.IService;

namespace LMS.BookService.Domain;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<Book> AddBookAsync(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        book.ModifiedAt = DateTime.UtcNow;
        book.Availability = Availability.Available;
        return await _bookRepository.AddBookAsync(book);
    }

    public async Task<Book> UpdateBookAsync(int id, Book book)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);

        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }
        
        foundBook.Author = book.Author;
        foundBook.Title = book.Title; 
        foundBook.Publisher = book.Publisher; 
        foundBook.CreatedAt = DateTime.UtcNow;
        foundBook.Availability = book.Availability;
        foundBook.Description = book.Description;
        
        return await _bookRepository.UpdateBookAsync(foundBook);
    }

    public async Task<Book> DeleteBookAsync(int id)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);
        
        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }
        
        return await _bookRepository.DeleteBook(foundBook);
    }

    public async Task<IEnumerable<Book>> GetBooksByPublishedDate(DateTime startDate, DateTime endDate)
    {
        var resultList = await _bookRepository.GetBooksFilteredAsync((book) => book.PublishDate >= startDate && book.PublishDate <= endDate);

        if (!resultList.Any())
        {
            throw new NotFoundDataException("There are no books with the published date"); 
        }

        return resultList.ToList();
    }

    public async Task<IEnumerable<Book>> GetBooksByAvailability(Availability availability)
    {
        var resultList = await _bookRepository.GetBooksFilteredAsync((book) => book.Availability == availability);

        if (!resultList.Any())
        {
            throw new NotFoundDataException("There are no books with this availability");
        }
        
        return resultList.ToList();
    }

    public async Task<IEnumerable<Book>> GetBooksByTitle(string title)
    {
        var resultList = await _bookRepository.GetBooksFilteredAsync((book) => book.Title.StartsWith(title));

        if (!resultList.Any())
        {
            throw new NotFoundDataException("There are no books with this title");
        }
        
        return resultList.ToList();
    }
}