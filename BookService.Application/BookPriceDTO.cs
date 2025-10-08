using BookService.Domain.Enum;

namespace BookService.Application;

public class BookPriceDTO
{
    public int BookId { get; set; }
    public BookType BookType { get; set; }
}