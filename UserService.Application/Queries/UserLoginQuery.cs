using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;
using Shared.Exception;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace UserService.Application.Queries;

public record UserLoginQuery(string Username, string Password) : IRequest<string>;

public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, string>
{
    private IConfiguration  _configuration;
    private readonly IUserRepository _userRepository;

    public UserLoginQueryHandler(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }


    public async Task<string> Handle(UserLoginQuery request, CancellationToken cancellationToken)
    {
        var foundUser = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (foundUser.Password != request.Password)
        {
            throw new UnauthenticationUserException("Invalid username or password");
        }
        
        return GenerateJwtToken(foundUser);
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Address, user.Address),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("role", user.RoleId.ToString()),
            new Claim("status", user.Status.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credential
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}