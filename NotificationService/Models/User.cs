namespace NotificationService.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public ICollection<UserNotiSubscription> UserNotiSubscriptions { get; set; } 
    public ICollection<NotificationStatus> NotificationStatuses { get; set; }
    
    public User() {}

    public User(string username, string email, string phoneNumber)
    {
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}