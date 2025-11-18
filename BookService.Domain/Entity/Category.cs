namespace BookService.Domain.Model;

public class Category : Entity
{
    public string Name { get; private set; }
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    
    public Category() {}

    public Category(string name)
    {
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}