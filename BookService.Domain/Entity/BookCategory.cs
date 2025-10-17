namespace BookService.Domain.Model;

public class BookCategory
{
    public int BookId { get; private set; }
    public Book? Book { get; private set; }
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public BookCategory()
    {
    }

    public BookCategory(int bookId, int categoryId)
    {
        BookId = bookId;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }

    public BookCategory(Book book, Category category)
    {
        Book = book;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }
    
}