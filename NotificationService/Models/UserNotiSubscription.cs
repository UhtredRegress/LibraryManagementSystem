namespace NotificationService.Models;

public class UserNotiSubscription
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int BookCategoryId { get; set; }
    public BookCategory BookCategory { get; set; } 
    
    public DateTime CreatedAt { get; set; }
    public UserNotiSubscription() { }

    public UserNotiSubscription(User user, BookCategory bookCategory)
    {
        UserId = user.Id;
        User = user;
        BookCategoryId = bookCategory.Id;
        BookCategory = bookCategory;
        CreatedAt = DateTime.UtcNow;
    }
}