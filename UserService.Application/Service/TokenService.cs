using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.Interface;

namespace LMS.Bussiness.Service;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public TokenService(ILogger<TokenService> logger, IUserRepository userRepository, IConfiguration config)
    {
        _logger = logger;
        _userRepository = userRepository;
        _config = config;
    }
    
    public async Task<string> RefreshAccessTokenAsync(int userId, string refreshToken)
    {
        _logger.LogInformation("Refreshing access token at Service Layer - {time}", DateTime.UtcNow.ToString("HH:mm:ss"));
        
        _logger.LogInformation("Start retrieve user in database with {userId}", userId);
        var foundUser = await _userRepository.GetUserByIdAsync(userId);
        if (foundUser == null)
        {
            _logger.LogInformation("User with id {userId} not found in database throw exception", userId);
            throw new InvalidDataException("User is not existed in the database");
        }
        
        _logger.LogInformation("User has found. Start validate refresh token");

        if (foundUser.TokenExpiredDateTime < DateTime.UtcNow)
        {
            _logger.LogInformation("Refresh token has been expired throwing exception");
            throw new InvalidDataException("Refresh token has been expired. Please log in again");
        }

        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        var hashString = Convert.ToHexString(hash);

        if (hashString != foundUser.HashedToken)
        {
            _logger.LogInformation("Refresh token is not valid throwing exception");
            throw new InvalidDataException("Refresh token is not valid");
        }
        
        _logger.LogInformation("Refresh token is valid.");

        _logger.LogInformation("Validate jwt key from configuration");
        if (string.IsNullOrEmpty(_config["Jwt:Key"]))
        {
            _logger.LogInformation("JWT key is empty from configuration throwing exception");
            throw new InvalidDataException("JWT key is empty");
        }

        if (string.IsNullOrEmpty(_config["Jwt:Issuer"]))
        {
            _logger.LogInformation("JWT issuer is empty from configuration throwing exception");
            throw new InvalidDataException("JWT issuer is empty");
        }

        if (string.IsNullOrEmpty(_config["Jwt:Audience"]))
        {
            _logger.LogInformation("JWT audience is empty from configuration throwing exception");
            throw new InvalidDataException("JWT audience is empty");
        }
        
        _logger.LogInformation("Required information is valid start generate access token");
        var accessToken = JwtTokenIssueHelper.GenerateJwtToken(foundUser, _config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"]);
        _logger.LogInformation("Generated access token at Service Layer is successfully - {time}" , DateTime.UtcNow.ToString("HH:mm:ss"));
        return accessToken; 
    }
}