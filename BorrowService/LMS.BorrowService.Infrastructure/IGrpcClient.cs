using Lirabry.Grpc;
using LMS.BorrowService.Domain.Entity;

namespace LMS.BorrowService.Infrastructure;

public interface IGrpcClient
{
    Task<bool> RequestCheckExistedBook(IEnumerable<int> request); 
    Task<Book> RequestFindBook(BookInfoRequest request);
}