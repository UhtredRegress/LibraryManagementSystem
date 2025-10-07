using BookService.Application.IService;
using BookService.Domain.Enum;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Shared.Exception;

namespace BookService.Application;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    public BookService(IBookRepository bookRepository, IMinioService minioService)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
    }


    public async Task<Book> AddBookAsync(Book book)
    {
        Book addedBook = new Book(book.Title, book.Author, book.Stock, book.Availability, book.PublishDate, book.Description,
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
        var foundBookList = await _bookRepository.GetRangeBookByIdAsync(bookId);
        
        foreach (var book in foundBookList)
        {
            book.BookBorrowed(); 
        }
        
        await _bookRepository.UpdateRangeBookAsync(foundBookList);
        
        return foundBookList; 
    }

    public async Task<Book> AddFileForBook(Book book, IFormFile file)
    {
        await _minioService.UploadFileAsync(file, book.Id.ToString() + "_" + book.Title + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        book.AddFileToBook(book.Id.ToString());
        return await _bookRepository.UpdateBookAsync(book);
    }

    public async Task<Book> AddFileForBookId(int id, IFormFile file)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);

        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }

        if (!string.IsNullOrEmpty(foundBook.FileName))
        {
            await _minioService.DeleteFileAsync(foundBook.FileName);
        }
        
        await _minioService.UploadFileAsync(file, foundBook.Id.ToString() + "_" + foundBook.Title + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        foundBook.AddFileToBook(foundBook.Id.ToString());
        return await _bookRepository.UpdateBookAsync(foundBook);
    }
}