using BookService.Application.IService;
using BookService.Domain.Enum;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Shared.Exception;

namespace BookService.Application;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<Book> AddBookAsync(Book book)
    {
        Book addedBook = new Book(book.Title, book.Author, book.Availability, book.PublishDate, book.Description,
            book.Description);
        
        return await _bookRepository.AddBookAsync(addedBook);
    }

    public async Task<Book> UpdateBookAsync(int id, Book book)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);

        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }

        foundBook.UpdateBook(book); 
        
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

    public async Task<IEnumerable<Book>> UpdateRangeBooksAsync(IEnumerable<int> bookId)
    {
        ICollection<Book> bookList = new List<Book>();
        foreach (var id in bookId)
        {
            bookList.Add(await _bookRepository.GetBookByIdAsync(id)); 
        }

        foreach (var book in bookList)
        {
            book.UpdateAvailability(Availability.Borrowed); 
        }

        foreach (var book in bookList)
        {
            await _bookRepository.UpdateBookAsync(book);
        }

        return bookList; 
    }
    
}