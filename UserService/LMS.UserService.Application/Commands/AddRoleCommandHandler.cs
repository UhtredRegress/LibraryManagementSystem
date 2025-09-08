using LMS.Shared.DTOs;
using LMS.UserService.Domain.Model;
using LMS.UserService.Infrastructure.Interface;
using MediatR;

namespace LMS.UserService.Application.Commands;

public record AddRoleCommand(RoleDTO RoleDTO) : IRequest<Role>;

public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, Role>
{
    private readonly IRoleRepository _roleRepo;

    public AddRoleCommandHandler(IRoleRepository roleRepo)
    {
        _roleRepo = roleRepo;
    }
    public async Task<Role> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        
        var mappingRole = new Role();
        mappingRole.Title = request.RoleDTO.Title;
        mappingRole.Id = await GenerateRoleId();
        mappingRole.CreatedAt = DateTime.UtcNow;
        mappingRole.ModifiedAt = DateTime.UtcNow;

        return await _roleRepo.AddRoleAsync(mappingRole);
    }
    
    private async Task<int> GenerateRoleId()
    {
        return (int) Math.Pow(2, await _roleRepo.GetRoleCount());
    }
}