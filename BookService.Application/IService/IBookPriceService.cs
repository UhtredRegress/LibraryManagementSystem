using BookService.Domain.Model;

namespace BookService.Application.IService;

public interface IBookPriceService
{
    Task<BookPrice> AddBookPriceAsync(BookPriceDTO bookPriceDTO);
}