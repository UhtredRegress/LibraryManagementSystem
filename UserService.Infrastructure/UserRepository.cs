using Microsoft.EntityFrameworkCore;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;

namespace UserService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user); 
        await _dbContext.SaveChangesAsync();
        return user;
    }
    
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}