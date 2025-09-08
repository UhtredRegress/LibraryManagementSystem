using FluentValidation;
using LMS.UserService.Application.Commands;
using LMS.UserService.Infrastructure.Interface;

namespace LMS.UserService.Application.Validator;

public class AddUserCommandValidator: AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        RuleFor(u => u.User.Username)
            .NotEmpty().WithMessage("Username is required")
            .MustAsync(async (username, _) => await userRepository.GetUserByUsernameAsync(username) == null).WithMessage("Username is already exist");
        RuleFor(u => u.User.RoleId)
            .NotEmpty().WithMessage("RoleId is required")
            .MustAsync(async (roleId, _) =>
            {
                var validRoleId = (int)Math.Pow(2, await roleRepository.GetRoleCount() + 1); 
                return roleId < validRoleId;
            }).WithMessage("RoleId is not valid");
    }
}