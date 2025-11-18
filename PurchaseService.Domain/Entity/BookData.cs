namespace PurchaseService.Domain.Entity;

public class BookData
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int BookType  { get; private set; }
    public decimal Price { get; private set; }

    public BookData(int id, string title, string author, int bookType, decimal price)
    {
        Id = id;
        Title = title;
        Author = author;
        BookType = bookType;
        Price = price;
    }
}