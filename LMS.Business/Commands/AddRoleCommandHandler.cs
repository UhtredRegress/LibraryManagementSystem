using LMS.Domain.Model;
using LMS.Infrastructure.Interface;
using LMS.Shared.DTOs;
using MediatR;

namespace LMS.Business.Commands;

public record AddRoleCommand(RoleDTO roleDTO) : IRequest<Role>;

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
        mappingRole.Title = request.roleDTO.Title;
        mappingRole.Id = await GenerateRoleId();

        return await _roleRepo.AddRoleAsync(mappingRole);
    }
    
    private async Task<int> GenerateRoleId()
    {
        return (int) Math.Pow(2, await _roleRepo.GetRoleCount());
    }
}