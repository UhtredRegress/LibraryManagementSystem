using Microsoft.EntityFrameworkCore;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;

namespace UserService.Infrastructure;

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

    public async Task<Role> UpdateRoleAsync(Role role)
    {
        _dbContext.Roles.Update(role);
        await _dbContext.SaveChangesAsync();
        return role;
    }

    public async Task<bool> DeleteRoleAsync(Role role)
    {
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}