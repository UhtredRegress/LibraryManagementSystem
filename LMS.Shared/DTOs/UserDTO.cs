namespace LMS.Shared.DTOs;

public class UserDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; } 
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int? Role { get; set; }
}