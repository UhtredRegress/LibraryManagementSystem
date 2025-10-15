using BookService.Application.IService;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Enum;
using Shared.Exception;

namespace BookService.Application;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly ILogger<BookService> _logger;
    private readonly IRepository<Author> _authorRepository;
    public BookService(IBookRepository bookRepository, IMinioService minioService, ILogger<BookService> logger, IRepository<Author> authorRepository)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
        _logger = logger;
        _authorRepository = authorRepository;
    }


    public async Task<Book> AddBookAsync(BookAddDTO bookAddDTO)
    {
        var typeCount = Enum.GetNames(typeof(BookType)).Length;
        var maximumType = 1 << typeCount;
        if (bookAddDTO.Title == null)
        {
            throw new InvalidDataException("Title is required.");
        }

        if (bookAddDTO.Type <= 0)
        {
            throw new InvalidDataException("Book Type is required.");
        }
        
        if (bookAddDTO.Type >= maximumType)
        {
            throw new InvalidDataException("Book type is not valid.");
        }
        

        if ((bookAddDTO.Type & (int)BookType.Physical) != 0)
        {
            if (bookAddDTO.Stock <= 0)
            {
                throw new InvalidDataException("Physical book require quantity in the system");
            }
        }
        
        if ((bookAddDTO.Type & (int)BookType.Ebook) != 0)
        {
            if (bookAddDTO.File == null || bookAddDTO.File.Length <= 0 )
            {
                throw new InvalidDataException("Ebook file is empty to create"); 
            }
        }
        
        var foundAuthor = await _authorRepository.GetRangeFilterAsync(a => bookAddDTO.Author.Contains(a.Id));
        
        var newBook = 
            Book.CreateBook(title: bookAddDTO.Title, authors: foundAuthor, description: bookAddDTO.Description, type: bookAddDTO.Type, publisher: bookAddDTO.Publisher, publishedDate: bookAddDTO.PublishDate, stock: bookAddDTO.Stock);

        var savedBook = await _bookRepository.AddBookAsync(newBook);

        if ((bookAddDTO.Type & (int)BookType.Ebook) != 0)
        {
            var fileName = $"{savedBook.Id}_{savedBook.Title}_{savedBook.CreatedAt.ToString("yyyyMMddHHmmss")}";
            savedBook.UpdateFileAddress(fileName); 
            await _minioService.UploadFileAsync(bookAddDTO.File, fileName);
            return await _bookRepository.UpdateBookAsync(newBook);
        }
        
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

        if (((int)foundBook.Type & (int)BookType.Ebook) == 0)
        {
            throw new InvalidDataException($"Book type {foundBook.Type} is not valid to update file");
        }
        if (!string.IsNullOrEmpty(foundBook.FileAddress))
        {
            try
            {
                await _minioService.DeleteFileAsync(foundBook.FileAddress);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        var fileName = foundBook.Id.ToString() + "_" + foundBook.Title + "_" +
                       DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        foundBook.UpdateFileAddress(fileName);
        await _minioService.UploadFileAsync(file, fileName);
        return await _bookRepository.UpdateBookAsync(foundBook);
    }
    
}