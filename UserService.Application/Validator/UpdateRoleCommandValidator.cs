using FluentValidation;
using UserService.Application.Commands;

namespace LMS.Bussiness.Validator;

public class UpdateRoleCommandValidator:AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than zero");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty");
    }
}