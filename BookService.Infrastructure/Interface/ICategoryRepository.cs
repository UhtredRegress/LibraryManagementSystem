using BookService.Domain.Model;
using BookService.Infrastructure.Interface;

namespace BookService.Infrastructure;

public interface ICategoryRepository : IRepository<Category>
{
    
}