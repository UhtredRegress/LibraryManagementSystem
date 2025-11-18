using System.Security.Cryptography;
using System.Text;
using LMS.Bussiness;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using UserService.Infrastructure.Interface;
using Shared.Exception;


namespace UserService.Application.Queries;

public record UserLoginQuery(string Username, string Password) : IRequest<LoginReponseDTO>;

public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, LoginReponseDTO>
{
    private IConfiguration  _configuration;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserLoginQueryHandler> _logger;

    public UserLoginQueryHandler(IUserRepository userRepository, IConfiguration configuration, ILogger<UserLoginQueryHandler> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }


    public async Task<LoginReponseDTO> Handle(UserLoginQuery request, CancellationToken cancellationToken)
    {
        var foundUser = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (foundUser.Password != request.Password)
        {
            throw new UnauthenticationUserException("Invalid username or password");
        }

        if (string.IsNullOrEmpty(_configuration["Jwt:Key"]))
        {
            _logger.LogInformation("Key for sign jwt is not available in the configuration throwing exception");
            throw new InvalidOperationException("Jwt key is not available in the configuration");
        }
        
        if (string.IsNullOrEmpty(_configuration["Jwt:Audience"]))
        {
            _logger.LogInformation("Jwt Audience is not available in the configuration throwing exception");
            throw new InvalidOperationException("Jwt audience is not available in the configuration");
        }

        if (string.IsNullOrEmpty(_configuration["Jwt:Issuer"]))
        {
            _logger.LogInformation("Jwt issuer is not available in the configuration throwing exception");
            throw new InvalidOperationException("Jwt issuer is not available in the configuration");
        }
        
        
        string accessToken = JwtTokenIssueHelper.GenerateJwtToken(foundUser, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"] );
        
        string refreshToken = Guid.NewGuid().ToString();
        
        var result = new LoginReponseDTO()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        refreshToken = Convert.ToHexString(hash);
        
        foundUser.UpdateTokenHashed(refreshToken);
        await _userRepository.UpdateUserAsync(foundUser);
        
        
        return result; 
    }
    
}