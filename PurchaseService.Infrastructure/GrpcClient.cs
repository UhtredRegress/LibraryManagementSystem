using Grpc.Net.Client;
using Lirabry.Grpc;

namespace PurchaseService.Infrastructure;

public class GrpcClient : IGrpcClient
{
    public async Task<Book?> RetrieveBookPriceCall(int bookId, int bookType)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:7080");
        var client = new BookAPI.BookAPIClient(channel);
        var request = new RetrieveBookPriceRequest { BookId = bookId, BookType = bookType };
        
        var response = await client.RetrieveBookPriceAsync(request);

        if (response.IsSuccess == false)
        {
            return null;
        }

        var result = new Book();
        result.BookId = response.Book.BookId;
        result.Title = response.Book.Title;
        result.Author = response.Book.Author;
        result.Publisher = response.Book.Publisher;
        result.PricePerUnit = response.Book.PricePerUnit;

        return result;
    }
}