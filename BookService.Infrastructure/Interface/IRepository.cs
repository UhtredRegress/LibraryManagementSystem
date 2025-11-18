using System.Linq.Expressions;

namespace BookService.Infrastructure.Interface;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
    Task<IEnumerable<TEntity>> GetRangeFilterAsync(Expression<Func<TEntity, bool>> filter);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> filter);
}