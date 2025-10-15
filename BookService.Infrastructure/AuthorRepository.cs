using BookService.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public sealed class AuthorRepository : Repository<Author>
{
    private readonly DbSet<Author> _dbSet;
    public AuthorRepository(BookServiceDbContext context) : base(context)
    {
        _dbSet = context.Set<Author>();
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync()
    {
        return await _dbSet.ToListAsync();
    }
}