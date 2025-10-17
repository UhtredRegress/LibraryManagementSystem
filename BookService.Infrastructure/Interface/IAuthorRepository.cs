using BookService.Domain.Model;
using BookService.Infrastructure.Interface;

namespace BookService.Infrastructure;

public interface IAuthorRepository : IRepository<Author>
{
    Task<IEnumerable<Author>> GetAllAuthorAsync();
}
