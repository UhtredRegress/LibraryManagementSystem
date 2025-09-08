using LMS.UserService.Domain.Model;

namespace LMS.UserService.Infrastructure.Interface;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
}