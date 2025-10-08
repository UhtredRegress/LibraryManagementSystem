using PurchaseService.Domain.Enum;

namespace PurchaseService.Domain.Aggregate;

public class PurchaseBook
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Username { get; private set; }
    public int BookId { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int BookType { get; private set; }
    public decimal FinalCost { get; private set; }
    public Status Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public PurchaseBook() { }

    public PurchaseBook(int userId, string username, int bookId, string title, string author, decimal finalCost, Status status, int bookType)
    {
        UserId = userId;
        Username = username;
        BookId = bookId;
        Title = title;
        Author = author;
        FinalCost = finalCost;
        CreatedAt = DateTime.UtcNow;
        Status = status;
        BookType = bookType;
    }

    public static PurchaseBook CreatePurchase(int userId, string username, int bookId, string title, string author,
        decimal finalCost, int bookType)
    {
        return new PurchaseBook(userId, username, bookId, title, author, finalCost, Status.Pending, bookType);
    }
    
}