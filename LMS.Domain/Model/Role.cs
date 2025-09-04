using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.Model;

public class Role
{
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("title")]
    public  string Title { get; set; }
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Required]
    [Column("modified_at")]
    public DateTime ModifiedAt { get; set; }
}