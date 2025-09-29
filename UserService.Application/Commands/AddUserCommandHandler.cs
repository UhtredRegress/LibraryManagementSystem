using MediatR;
using UserService.Domain.Model;
using UserService.Infrastructure.Interface;
using Shared.DTOs;

namespace UserService.Application.Commands;

public record AddUserCommand : IRequest<UserDTO>
{
    public User User { get; set; }
}

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, UserDTO>
{
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    { 
        request.User.CreatedAt = DateTime.UtcNow;
        request.User.ModifiedAt = DateTime.UtcNow;
        
        User result = await _userRepository.AddUserAsync(request.User);
        
        UserDTO userDTO = new UserDTO();
        userDTO.Address = result.Address;
        userDTO.Email = result.Email;
        userDTO.PhoneNumber = result.PhoneNumber;
        userDTO.Username = result.Username;
        userDTO.RoleId = result.RoleId;

        return userDTO;
    }
}