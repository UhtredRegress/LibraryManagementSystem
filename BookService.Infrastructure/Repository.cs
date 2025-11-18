using System.Linq.Expressions;
using BookService.Domain;
using BookService.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly BookServiceDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(BookServiceDbContext context)
    {
        _dbSet = context.Set<TEntity>();
        _context = context;
    }
    
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        var result = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetRangeFilterAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbSet.Where(filter).ToListAsync();
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbSet.FirstOrDefaultAsync(filter);
    }
}