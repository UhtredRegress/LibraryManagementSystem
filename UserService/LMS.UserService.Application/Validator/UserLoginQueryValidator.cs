using FluentValidation;
using LMS.UserService.Infrastructure.Interface;
using LMS.UserService.Queries;

namespace LMS.UserService.Application.Validator;

public class UserLoginQueryValidator : AbstractValidator<UserLoginQuery>
{
    public UserLoginQueryValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MustAsync(async (username, _) => await userRepository.GetUserByUsernameAsync(username) != null)
            .WithMessage("Username is not existed");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}