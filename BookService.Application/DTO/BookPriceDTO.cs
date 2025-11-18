using Shared.Enum;

namespace BookService.Application;

public class BookPriceDTO
{
    public int BookId { get; set; }
    public BookType BookType { get; set; }
    public decimal PricePerUnit { get; set; }
}