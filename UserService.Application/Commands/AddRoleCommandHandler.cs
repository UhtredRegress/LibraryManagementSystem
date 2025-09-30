using MediatR;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;
using Shared.DTOs;

namespace UserService.Application.Commands;

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
        mappingRole.Id = (int) Math.Pow(2, await _roleRepo.GetRoleCount());
        mappingRole.CreatedAt = DateTime.UtcNow;
        mappingRole.ModifiedAt = DateTime.UtcNow;

        return await _roleRepo.AddRoleAsync(mappingRole);
    }
}