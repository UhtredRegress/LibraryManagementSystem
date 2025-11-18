using System.Linq.Expressions;
using PurchaseService.Domain.Aggregate;

namespace PurchaseService.Infrastructure;

public interface IPurchaseRepository
{
    Task<PurchaseBook> AddPurchaseBookAsync(PurchaseBook purchaseBook);
    Task<PurchaseBook> UpdatePurchaseBookAsync(PurchaseBook book);
    Task<PurchaseBook?> GetPurchaseBookAsync(int id);
    Task<PurchaseBook> DeletePurchaseBookAsync(PurchaseBook purchaseBook);
    Task<IEnumerable<PurchaseBook>> GetRangePurchaseBooksFilterAsync(Expression<Func<PurchaseBook, bool>> predicate);
    Task<PurchaseBook?> GetPurchaseBookFilteredAsync(Expression<Func<PurchaseBook, bool>> predicate);
}