namespace BookService.Domain;

public abstract class Entity
{
    public int Id { get; private set; }

    public Entity()
    {
    }

    public Entity(int id)
    {
        Id = id;
    }
}