using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure;

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
}