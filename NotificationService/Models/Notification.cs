namespace NotificationService.Models;

public class Notification
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt{ get; set; }
    
    public ICollection<NotificationStatus> NotificationStatuses { get; set; }
    
    public Notification() { }

    public Notification(string content)
    {
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
    
}