using LMS.UserService.Domain.Model;

namespace LMS.UserService.Infrastructure.Interface;

public interface IRoleRepository
{
    Task<Role> AddRoleAsync(Role role);
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role?> GetRoleByTitleAsync(string title);
    Task<int> GetRoleCount();
}