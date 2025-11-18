using BookService.Application.IService;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;
using Shared.Enum;

namespace BookService.Application;

public class BookPriceService : IBookPriceService
{
    private readonly IBookPriceRepository _bookPriceRepository;
    private readonly IBookRepository _bookRepository;

    public BookPriceService(IBookPriceRepository bookPriceRepository, IBookRepository bookRepository)
    {
        _bookPriceRepository = bookPriceRepository;
        _bookRepository = bookRepository;
    }
    public async Task<BookPrice> AddBookPriceAsync(BookPriceDTO bookPriceDTO)
    {
        if (bookPriceDTO.BookType == null)
        {
            throw new InvalidOperationException("BookType cannot be null");
        }
        
        bool checkValid = Enum.IsDefined(typeof(BookType), bookPriceDTO.BookType);
        if (checkValid == false)
        {
            throw new InvalidOperationException("BookType is not valid");
        }

        if (bookPriceDTO.PricePerUnit <= 0)
        {
            throw new InvalidOperationException("Price per unit must be positive");
        }
        
        var foundBook = await _bookRepository.GetBookByIdAsync(bookPriceDTO.BookId);

        if (foundBook == null)
        {
            throw new InvalidOperationException("Book not found");
        }

        if (((int)foundBook.Type & (int)bookPriceDTO.BookType) == 0)
        {
            throw new InvalidOperationException("The book does not contains this type");
        }
        
        BookPrice bookPrice = new BookPrice(bookId: bookPriceDTO.BookId, bookType: bookPriceDTO.BookType, priceUnit: bookPriceDTO.PricePerUnit);

        var saveBookPrice = await _bookPriceRepository.AddBookPriceAsync(bookPrice);
        return saveBookPrice;
    }
}