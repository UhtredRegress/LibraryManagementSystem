namespace BookService.Domain.Model;

public class Author : Entity
{
    public string Name { get; private set; }
    
    public ICollection<Book> Books { get; private set; } = new List<Book>();
    
    public Author() {}

    public Author(int id, string name) : base(id)
    {
        Name = name;
    }

    public Author(string name)
    {
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}