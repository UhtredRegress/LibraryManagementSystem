using BookService.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public sealed class AuthorRepository : Repository<Author>, IAuthorRepository
{
    private readonly DbSet<Author> _dbSet;
    public AuthorRepository(BookServiceDbContext context) : base(context)
    {
        _dbSet = context.Set<Author>();
    }
    public async Task<IEnumerable<Author>> GetAllAuthorAsync()
    {
        return await _dbSet.ToListAsync();
    }
}