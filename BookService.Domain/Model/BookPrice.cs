using BookService.Domain.Enum;

namespace BookService.Domain.Model;

public class BookPrice
{
    public int BookId { get; private set; }
    public BookType BookType { get; private set; }
    public decimal PriceUnit { get; private set; }
    
    public BookPrice() { }

    public BookPrice(int bookId, BookType bookType, decimal priceUnit)
    {
        BookId = bookId;
        BookType = bookType;
        PriceUnit = priceUnit;
    }
}