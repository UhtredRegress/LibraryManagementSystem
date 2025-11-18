using BorrowService.Domain.Entity;
using Lirabry.Grpc;

namespace BorrowService.Infrastructure;

public class GrpcClient : IGrpcClient
{
    private BookAPI.BookAPIClient  _client; 
    public GrpcClient(BookAPI.BookAPIClient client)
    {
        _client = client;
    }
    public async Task<bool> RequestCheckExistedBook(IEnumerable<int> request)
    {
        var grpcRequest = new CheckExistedBookRequest();
        grpcRequest.BookId.AddRange(request);
        
        var response = await _client.CheckExistAsync(grpcRequest);
        return response.Result;
    }

    public Task<Book> RequestFindBook(BookInfoRequest request)
    {
        throw new NotImplementedException();
    }
}