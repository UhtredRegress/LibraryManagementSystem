using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Exception;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;

namespace UserService.Application.Commands;

public record UpdateRoleCommand(int Id, string Title) : IRequest<Role>;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Role>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    public UpdateRoleCommandHandler(IRoleRepository roleRepository, ILogger<UpdateRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }
    
    public async Task<Role> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating role {id} with title {title}", request.Id, request.Title);
        var foundRole = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (foundRole == null)
        {
            throw new NotFoundDataException();
        }
        
        foundRole.Update(request.Title);
        var result = await _roleRepository.UpdateRoleAsync(foundRole);
        _logger.LogInformation("Role {id} updated", request.Id);

        return result; 
    }
}