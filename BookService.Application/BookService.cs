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


    public async Task<Book> AddBookAsync(Book book, IFormFile file)
    {
        var typeCount = Enum.GetNames(typeof(BookType)).Length;
        var maximumType = 1 << typeCount;
        if (book.Title == null)
        {
            throw new InvalidDataException("Title is required.");
        }
        
        if ((int)book.Type >= maximumType)
        {
            throw new InvalidDataException($"Book type {book.Type} is too high.");
        }

        if (book.Type <= 0)
        {
            throw new InvalidDataException($"Book type {book.Type} is negative.");
        }

        if (((int)book.Type & (int)BookType.Ebook) != 0)
        {
            if (file.Length <= 0 || file == null)
            {
                throw new InvalidDataException("Ebook file is empty to create"); 
            }
        }

        if (((int)book.Type & (int)BookType.Physical) != 0)
        {
            if (book.Stock <= 0)
            {
                throw new InvalidDataException("Physical book require quantity in the system");
            }
        }
        
        var newBook = 
            Book.CreateBook(title: book.Title, author: book.Author, description: book.Description, type: book.Type, publisher: book.Publisher, publishedDate: book.PublishDate, fileAddress: book.FileAddress, stock: book.Stock);

        var savedBook = await _bookRepository.AddBookAsync(newBook);

        var fileName = $"{savedBook.Id}_{savedBook.Title}_{savedBook.CreatedAt.ToString("yyyyMMddHHmmss")}";
        await _minioService.UploadFileAsync(file, fileName);
        return savedBook;
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
    

    public async Task<Book> UpdateFileForBookId(int id, IFormFile file)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);

        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }

        if (((int)foundBook.Type & (int)BookType.Ebook) != 0)
        {
            throw new InvalidDataException($"Book type {foundBook.Type} is not valid to update file");
        }
        if (!string.IsNullOrEmpty(foundBook.FileAddress))
        {
            await _minioService.DeleteFileAsync(foundBook.FileAddress);
        }

        var fileName = foundBook.Id.ToString() + "_" + foundBook.Title + "_" +
                       DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        await _minioService.UploadFileAsync(file, fileName);
        return await _bookRepository.UpdateBookAsync(foundBook);
    }
}