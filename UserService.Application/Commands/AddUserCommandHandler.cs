using Domain.DomainEvent;
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
    private readonly IMediator _mediator;

    public AddUserCommandHandler(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }
    
    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    { 
        var createdUser = User.UserRegister(request.User.Username, request.User.Password, request.User.Email, request.User.PhoneNumber, request.User.Address, request.User.RoleId);
        
        var result = await _userRepository.AddUserAsync(createdUser);

        _mediator.Publish(new NewUserCreatedEvent(result.Id, result.Email, result.CreatedAt, result.Address));
        
        UserDTO userDTO = new UserDTO();
        userDTO.Address = createdUser.Address;
        userDTO.Email = createdUser.Email;
        userDTO.PhoneNumber = createdUser.PhoneNumber;
        userDTO.Username = createdUser.Username;
        userDTO.RoleId = createdUser.RoleId;

        return userDTO;
    }
}