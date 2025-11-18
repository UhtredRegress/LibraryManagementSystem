using BookService.Domain.Model;

namespace BookService.Infrastructure;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(BookServiceDbContext context) : base(context)
    {
    }
}