using UserService.Domain.Model;

namespace UserService.Infrastructure.Interface;

public interface IRoleRepository
{
    Task<Role> AddRoleAsync(Role role);
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role?> GetRoleByTitleAsync(string title);
    Task<int> GetRoleCount();
}