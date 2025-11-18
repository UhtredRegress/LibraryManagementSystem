namespace Shared.DTOs;

public class LoginDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class LoginReponseDTO
{
    public string AccessToken { get; set; } 
    public string RefreshToken { get; set; }
    public string TokenType { get; set; } 
    public int ExpiresIn { get; set; }
}