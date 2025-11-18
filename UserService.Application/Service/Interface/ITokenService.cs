namespace LMS.Bussiness.Service;

public interface ITokenService
{
    Task<string> RefreshAccessTokenAsync(int userId, string refreshToken);
}