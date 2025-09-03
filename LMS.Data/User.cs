public class User
{
    public int Id { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    public int RoleId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; } 
}