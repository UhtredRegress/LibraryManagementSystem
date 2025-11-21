namespace NotificationService.Models;

public class NotificationStatus
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int NotificationId { get; set; }
    public Notification Notification { get; set; }

    public bool IsReaded { get; set; } = false;
    
    public NotificationStatus() { }

    public NotificationStatus(User user, Notification notification)
    {
        UserId = user.Id;
        User = user;
        NotificationId = notification.Id;
        Notification = notification;
    }
    
}