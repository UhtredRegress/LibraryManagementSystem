using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.Interface;

namespace UserService.Application.Commands;

public record DeleteRoleCommand(int RoleId) : IRequest<bool>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;
    
    public DeleteRoleCommandHandler(IRoleRepository roleRepository, ILogger<DeleteRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting role {roleId}", request.RoleId);
        var foundRole = await _roleRepository.GetRoleByIdAsync(request.RoleId);
        if (foundRole is null)
        {
            _logger.LogInformation($"No role found with id {request.RoleId}");
            return false;
        }
        
        var result = await _roleRepository.DeleteRoleAsync(foundRole);
        _logger.LogInformation("Role {roleId} deleted", request.RoleId);
        return true;
    }
}