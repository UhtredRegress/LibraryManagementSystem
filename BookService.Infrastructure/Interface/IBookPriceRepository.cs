using System.Linq.Expressions;
using BookService.Domain.Model;

namespace BookService.Infrastructure.Interface;

public interface IBookPriceRepository
{
    Task<BookPrice?> GetBookPriceAsync(int bookId);
    Task<BookPrice> AddBookPriceAsync(BookPrice bookPrice);
    Task<BookPrice> UpdateBookPriceAsync(BookPrice bookPrice);
    Task<BookPrice> DeleteBookPriceAsync(BookPrice bookPrice);
    Task<BookPrice?> GetBookPriceFiltered(Expression<Func<BookPrice, bool>> predicate);
}