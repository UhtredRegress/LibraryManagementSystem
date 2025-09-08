
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LMS.UserService.Domain.Model;

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

    
}