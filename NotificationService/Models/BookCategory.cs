namespace NotificationService.Models;

public class BookCategory
{
    public int Id { get; set; } 
    public string  Name { get; set; }
    public ICollection<UserNotiSubscription> UserNotiSubscriptions { get; set; }
}