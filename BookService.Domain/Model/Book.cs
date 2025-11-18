using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookService.Domain.Model;

public class Book
{
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("title")]
    public string Title { get; set; }
    public ICollection<Author> Authors { get; private set; } = new List<Author>();
    public ICollection<BookCategory> BookCategories { get; private set; } = new List<BookCategory>();
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
    public int Type { get; private set; }
    public string? FileAddress { get; private set; }
    public int? Stock { get; private set; }
    
    public Book() {}

    public Book(Book other)
    {
        Id = other.Id;
        Title = other.Title;
        Authors = other.Authors;
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

    public static Book CreateBook(string title, IEnumerable<Author> authors, string description, string publisher,
        DateTime? publishedDate, int type, IEnumerable<Category> categories, string? fileAddress = null, int? stock = null)
    {
        var book = new Book();
        book.Id = 0;
        book.Title = title;
        book.Publisher = publisher;
        book.Description = description;
        book.PublishDate = publishedDate;
        book.FileAddress = fileAddress;
        book.Stock = stock;
        book.Type = type;
        book.Stock = stock;
        foreach (var author in authors)
        {
            book.Authors.Add(author);
        }

        foreach (var category in categories)
        {
            book.BookCategories.Add(new BookCategory(book, category));
        }
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

    public void UpdateFileAddress(string fileAddress)
    {
        FileAddress = fileAddress;
    }

    public void AddAuthor(Author author)
    {
        if (Authors.Contains(author))
        {
            return;
        }
        Authors.Add(author);
    }

    public void UpdateInformationBook(string title, string publisher, string description, DateTime? publishDate,
        IEnumerable<Author> authors, IEnumerable<Category> categories)
    {
        Title = title;
        Publisher = publisher;
        Description = description;
        PublishDate = publishDate;
        Authors = authors.ToList();
        BookCategories = new List<BookCategory>();
        foreach (var category in categories)
        {
            BookCategories.Add(new BookCategory(this, category));
        }
    }
    
}