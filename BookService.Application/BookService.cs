using BookService.Application.IService;
using BookService.Domain.Model;
using BookService.Infrastructure;
using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using Shared.Enum;
using Shared.Exception;

namespace BookService.Application;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly ILogger<BookService> _logger;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEventBus _eventBus;

    public BookService(IBookRepository bookRepository, IMinioService minioService, ILogger<BookService> logger,
        IAuthorRepository authorRepository, ICategoryRepository categoryRepository, IEventBus eventBus)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
        _logger = logger;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _eventBus = eventBus;
    }

    public async Task<Book> AddBookAsync(BookAddDTO bookAddDTO)
    {
        _logger.LogInformation("Start handling add new book");
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
            if (bookAddDTO.File == null || bookAddDTO.File.Length <= 0)
            {
                throw new InvalidDataException("Ebook file is empty to create");
            }
        }

        _logger.LogInformation("Retrieve all the author of the book in the request");
        var foundAuthor = await _authorRepository.GetRangeFilterAsync(a => bookAddDTO.Author.Contains(a.Id));

        if (foundAuthor.Count() < bookAddDTO.Author.Count())
        {
            _logger.LogInformation("There are author in the request that not exist in database ");
            throw new InvalidDataException("Author does not exist in database");
        }
        
        _logger.LogInformation("Retrieve all the category of the book in the request");
        var foundCategories = await _categoryRepository.GetRangeFilterAsync(c => bookAddDTO.Category.Contains(c.Id));

        if (foundCategories.Count() < bookAddDTO.Category.Count())
        {
            _logger.LogInformation("There are categories in the request that not exist in database");
            throw new InvalidDataException("Categories does not exist in database");
        }
        
        _logger.LogInformation("Started generate and store this data in the database");
        var newBook = Book.CreateBook(title: bookAddDTO.Title, authors: foundAuthor,
            description: bookAddDTO.Description, type: bookAddDTO.Type, publisher: bookAddDTO.Publisher,
            publishedDate: bookAddDTO.PublishDate, stock: bookAddDTO.Stock, categories: foundCategories );

        var savedBook = await _bookRepository.AddBookAsync(newBook);

        if ((bookAddDTO.Type & (int)BookType.Ebook) != 0)
        {
            _logger.LogInformation("Started stored ebook into storage");
            var fileName = $"{savedBook.Id}_{savedBook.Title}_{savedBook.CreatedAt.ToString("yyyyMMddHHmmss")}";
            
            await _minioService.UploadFileAsync(bookAddDTO.File, fileName);
            
            _logger.LogInformation("Started update file address {FileName} into database", fileName);
            savedBook.UpdateFileAddress(fileName);
            return await _bookRepository.UpdateBookAsync(newBook);
        }
        
        _logger.LogInformation("Persistence data successfully start emit integrated event");
        var integratedEvent = new NewBookCreatedIntegratedEvent {Title = savedBook.Title, Author = string.Join(", ", savedBook.Authors.Select(c => c.Name)), Category = string.Join(", ", savedBook.BookCategories.Select(c => c.Category?.Name))};
        //await _eventBus.PublishAsync(integratedEvent);
        
        QueueService.Queue.Enqueue(integratedEvent); 
        
        return savedBook;
    }

    public async Task<BookResultDTO> UpdateBookAsync(int id, BookUpdateInformationDTO book)
    {
        _logger.LogInformation("Started handle request to update the book id {BookId}", id);

        _logger.LogInformation("Find book id {BookId} in the database", id);
        var foundBook = await _bookRepository.GetBookByIdAsync(id);

        if (foundBook == null)
        {
            _logger.LogInformation("Book with id {BookId} not found in the database", id);
            throw new NotFoundDataException("The book does not exist");
        }

        _logger.LogInformation("Retrieve all the author of the book in the request");
        var foundAuthor = await _authorRepository.GetRangeFilterAsync(a => book.Author.Contains(a.Id));

        if (foundAuthor.Count() < book.Author.Count())
        {
            _logger.LogInformation("There are author in the request that not exist in database ");
            throw new InvalidDataException("There are author does not exist in database");
        }
        
        _logger.LogInformation("Retrieve all the category of the book in the request"); 
        var foundCategories = await _categoryRepository.GetRangeFilterAsync(a => book.Category.Contains(a.Id));
        if (foundCategories.Count() < book.Category.Count() )
        {
            _logger.LogInformation("There are categories in the request that not exist in database");
            throw new InvalidDataException("There are author does not exist in database");
        }

        _logger.LogInformation("Started update information in the database");
        foundBook.UpdateInformationBook(book.Title, book.Publisher, book.Description, book.PublishDate, foundAuthor, categories: foundCategories );
        await _bookRepository.UpdateBookAsync(foundBook);
        
        _logger.LogInformation("Started mapping the result to DTO and return to calling layer");
        return new BookResultDTO(foundBook.Id, foundBook.Title, foundBook.Authors, foundBook.BookCategories, foundBook.Publisher, foundBook.Type,
            foundBook.Stock, foundBook.FileAddress);
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
        var resultList =
            await _bookRepository.GetBooksFilteredAsync((book) =>
                book.PublishDate >= startDate && book.PublishDate <= endDate);

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

    public async Task<BookResultDTO> GetBookById(int id)
    {
        var foundBook = await _bookRepository.GetBookByIdAsync(id);
        if (foundBook == null)
        {
            throw new NotFoundDataException("The book does not exist");
        }

        return new BookResultDTO(foundBook.Id, foundBook.Title, foundBook.Authors, foundBook.BookCategories, foundBook.Publisher, foundBook.Type,
            foundBook.Stock, foundBook.FileAddress);
    }
}