using BorrowService.Domain.Entity;
using Lirabry.Grpc;

namespace BorrowService.Infrastructure;

public interface IGrpcClient
{
    Task<bool> RequestCheckExistedBook(IEnumerable<int> request); 
    Task<Book> RequestFindBook(BookInfoRequest request);
}