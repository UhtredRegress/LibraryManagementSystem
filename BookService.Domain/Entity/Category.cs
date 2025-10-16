namespace BookService.Domain.Model;

public class Category : Entity
{
    public string Name { get; set; }
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>()
}