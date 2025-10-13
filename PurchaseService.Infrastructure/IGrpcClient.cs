using Lirabry.Grpc;

namespace PurchaseService.Infrastructure;

public interface IGrpcClient
{
    Task<Book?> RetrieveBookPriceCall(int bookId, int bookType);
}