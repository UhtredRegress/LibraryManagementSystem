using Grpc.Core;
using Grpc.Net.Client;
using Library.Grpc;
using Microsoft.Extensions.Logging;

namespace BookService.Infrastructure;

public class GrpcClient : IGrpcClient
{
    private readonly ILogger<GrpcClient> _logger;

    public GrpcClient(ILogger<GrpcClient> logger)
    {
        _logger = logger;
    }
    public async Task<bool> CheckIfAlreadyBuyEbook(int userId, int bookId)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:7081");
        var client = new PurchaseGrpcAPI.PurchaseGrpcAPIClient(channel);
        
        var request = new FindSuccessfullyPurchaseEBookRequest() {UserId = userId, BookId = bookId};
        try
        {
            var result = await client.FindSuccessfullyPurchaseEbookAsync(request);
            return result.Result;
        }
        catch (RpcException rpcException)
        {
            _logger.LogError(rpcException.Message);
            return false;
        }
    }
    
}