using BookService.Application.IService;
using BookService.Domain.Enum;
using BookService.Domain.Model;
using BookService.Infrastructure.Interface;

namespace BookService.Application;

public class BookPriceService : IBookPriceService
{
    private readonly IBookPriceRepository _bookPriceRepository;

    public BookPriceService(IBookPriceRepository bookPriceRepository)
    {
        _bookPriceRepository = bookPriceRepository;
    }
    public Task<BookPrice> AddBookPriceAsync(BookPriceDTO bookPriceDTO)
    {
        if (bookPriceDTO.BookType == null)
        {
            throw new InvalidOperationException("BookType cannot be null");
        }
        
        var bookTypeCount = Enum.GetNames(typeof(BookType)).Length;
        var maximum = 1 << bookTypeCount;

        if ((int)bookPriceDTO.BookType >= maximum)
        {
            
        }
    }
}