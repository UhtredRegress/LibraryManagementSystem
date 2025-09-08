using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.BookService.Domain.Enum;

namespace LMS.BookService.Domain.Model;

public class Book
{
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("title")]
    public string Title { get; set; }
    [Column("author")]
    public string? Author { get; set; }
    [Required]
    [Column("availabily")]
    public Availability Availability { get; set; }
    public string Publisher { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("published_date")]
    public DateTime? PublishDate { get; set; }
    [Required]
    [Column("create_at")]
    public DateTime CreatedAt { get; set; }
    [Required]
    [Column("modified_at")]
    public DateTime ModifiedAt { get; set; }
    
}