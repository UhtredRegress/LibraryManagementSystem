using Domain.Enum;
using MediatR;
using Shared.Exception;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;
using UserService.Infrastructure.Service.Interface;

namespace UserService.Application.Commands;

public record ConfirmEmailCommand(string Id, string Email, string statusClaim, string Token) : IRequest<bool>;

public class ConfirmEmailCommandHandler  : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailTokenService _emailTokenService;
    public ConfirmEmailCommandHandler(IUserRepository userRepository, IEmailTokenService emailTokenService)
    {
        _userRepository = userRepository;
        _emailTokenService = emailTokenService;
    }
    
    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(request.Id, out var userId))
        {
            throw new InvalidDataException("Invalid user id");
        }

        User foundUser = await _userRepository.GetUserByIdAsync(userId);
        if (foundUser is null)
        {
            throw new NotFoundDataException("User is not found");
        } 
        
        var result = await _emailTokenService.ValidateToken(foundUser.Email, request.Token);
        if (result == false)
        {
            return false;
        }

        foundUser.ActivateUser(); 
        await _userRepository.UpdateUserAsync(foundUser);
        return true;
    }
}