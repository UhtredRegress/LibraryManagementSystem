using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Role> AddRoleAsync(Role role)
    {
        
        await _dbContext.Roles.AddAsync(role);
        await _dbContext.SaveChangesAsync();
        return role;
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetRoleByTitleAsync(string title)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Title == title);  
    }

    public async Task<int> GetRoleCount()
    {
        return await _dbContext.Roles.CountAsync();
    }
}