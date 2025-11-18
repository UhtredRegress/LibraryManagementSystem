using UserService.Domain.Model;

namespace UserService.Infrastructure.Interface;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> DeleteUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);
}