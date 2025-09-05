using FluentValidation;
using LMS.Business.Queries;
using LMS.Infrastructure.Interface;

namespace LMS.Business.Validator;

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