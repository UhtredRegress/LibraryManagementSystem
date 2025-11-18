namespace UserService.Infrastructure.Service.Interface;

public interface IEmailTokenService
{
    Task<string> GenerateToken(string email, int expireSeconds);
    Task<bool> ValidateToken(string email, string token);
}