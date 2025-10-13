using Shared.Enum;
using PurchaseStatus = PurchaseService.Domain.Enum.PurchaseStatus;

namespace PurchaseService.Domain.Aggregate;

public class PurchaseBook
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Username { get; private set; }
    public int BookId { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public BookType BookType { get; private set; }
    public int Amount { get; private set; }
    public decimal FinalCost { get; private set; }
    public PurchaseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? SessionUrl { get; private set; }
    
    public PurchaseBook() { }

    public PurchaseBook(int userId, string username, int bookId, string title, string author, decimal finalCost, PurchaseStatus status, BookType bookType, int amount, string? sessionUrl = null)
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
        Amount = amount;
        SessionUrl = sessionUrl;
    }

    public static PurchaseBook CreatePurchase(int userId, string username, int bookId, string title, string author,
        decimal finalCost, BookType bookType, int amount)
    {
        return new PurchaseBook(userId, username, bookId, title, author, finalCost, PurchaseStatus.Pending, bookType, amount);
    }

    public void UpdateSessionUrl(string sessionUrl)
    {
        SessionUrl = sessionUrl;
    }

    public void PurchaseSuccessfully()
    {
        Status = PurchaseStatus.Approved;
    }
}