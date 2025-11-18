using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.Interface;

namespace UserService.Application.Commands;

public record DeleteUserCommand(int Id) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository  _userRepository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with id {Id}", request.Id);
        var deleteUser = await _userRepository.GetUserByIdAsync(request.Id);
        if (deleteUser == null)
        {
            return false;
        }
        return await _userRepository.DeleteUserAsync(deleteUser);
    }
}