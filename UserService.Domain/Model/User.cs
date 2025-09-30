using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;


namespace UserService.Domain.Model;

public class User
{
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("password")]
    public string Password { get; set; }
    [Required]
    [Column("user_name")]
    public string Username { get; set; }
    [Column("role_id")]
    public int? RoleId { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Column("email")]
    public string Email { get; set; }
    [Required]
    [Column("phone_number")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; }
    [Required]
    [Column("address")]
    public string Address { get; set; }
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Required]
    [Column("modified_at")]
    public DateTime ModifiedAt { get; set; }
    
    public Status Status { get; private set; }

    public User() { }

    public User(string username, string password, string email, string phoneNumber, string address, int? roleId)
    {
        Username = username;
        Password = password;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        RoleId = roleId;
    }
    public User UpdateInformation(User updateUser)
    {
        Username = updateUser.Username;
        RoleId = updateUser.RoleId;
        Email = updateUser.Email;
        PhoneNumber = updateUser.PhoneNumber;
        Address = updateUser.Address;
        ModifiedAt = DateTime.UtcNow;
        Status = updateUser.Status;
        return this;
    }

    public static User UserRegister(string username, string password, string email, string phoneNumber, string address, int? roleId)
    {
        User user = new User(username, password, email, phoneNumber, address, roleId);
        user.CreatedAt = DateTime.UtcNow;
        user.ModifiedAt = DateTime.UtcNow;
        user.Status = Status.Inactive;
        return user;
    }

    public User ActivateUser()
    {
        Status = Status.Active;
        ModifiedAt = DateTime.UtcNow;
        return this;
    }
}