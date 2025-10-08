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
    public BookType Type { get; private set; }
    public string? FileAddress { get; private set; }
    public int? Stock { get; private set; }
    
    public Book() {}

    public Book(Book other)
    {
        Id = other.Id;
        Title = other.Title;
        Author = other.Author;
        Publisher = other.Publisher;
        Description = other.Description;
        PublishDate = other.PublishDate;
        CreatedAt = other.CreatedAt;
        ModifiedAt = other.ModifiedAt;
        Type = other.Type;
        FileAddress = other.FileAddress;
        Stock = other.Stock;
    }
    
    public void UpdateBook(Book book)
    {
        Id = book.Id;
        Title = book.Title;
        PublishDate = book.PublishDate;
        Description = book.Description;
        Publisher = book.Publisher;
        CreatedAt = book.CreatedAt;
        ModifiedAt = DateTime.UtcNow;
    }

    public static Book CreateBook(string title, string author, string description, string publisher,
        DateTime? publishedDate, BookType type, string? fileAddress = null, int? stock = null)
    {
        var book = new Book();
        book.Id = 0;
        book.Title = title;
        book.Author = author;
        book.Publisher = publisher;
        book.Description = description;
        book.PublishDate = publishedDate;
        book.FileAddress = fileAddress;
        book.Stock = stock;
        book.Type = type;
        book.FileAddress = fileAddress;
        book.Stock = stock;
        return book;
    }

    public void BookBorrowed()
    {
        Stock--;
    }

    public void BookReturned()
    {
        Stock++;
    }
}