namespace BookService.Infrastructure;

public interface IGrpcClient
{
    Task<bool> CheckIfAlreadyBuyEbook(int userId, int bookId);
}