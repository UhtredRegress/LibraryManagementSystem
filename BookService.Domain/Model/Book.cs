using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookService.Domain.Enum;

namespace BookService.Domain.Model;

public class Book
{
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("title")]
    public string? Title { get; set; }
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
    public int Stock { get; private set; }
    public string? FileName { get; private set; }
    
    public Book() {}

    public Book(string title, string author, int stock, Availability availability, DateTime? publishDate, string description,
        string publisher)
    {
        Title = title;
        Author = author;
        Availability = availability;
        PublishDate = publishDate;
        Description = description;
        Publisher = publisher;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        Stock = stock;
        FileName = null;
    }
    
    public void UpdateBook(Book book)
    {
        Id = book.Id;
        Title = book.Title;
        Availability = book.Availability;
        PublishDate = book.PublishDate;
        Description = book.Description;
        Publisher = book.Publisher;
        CreatedAt = book.CreatedAt;
        ModifiedAt = DateTime.UtcNow;
        Stock = book.Stock;
    }
    
    public void UpdateAvailability(Availability availability)
    {
        Availability = availability;
    }

    public void BookBorrowed()
    {
        Stock--;
    }

    public void BookReturned()
    {
        Stock++;
    }

    public void AddFileToBook(string title)
    {
        FileName = title;
    }
}