using FluentValidation;
using LMS.Business.Commands;
using LMS.Business.Validator;
using LMS.Infrastructure.Interface;

namespace LMS.Business.Validator;

public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleCommandValidator(IRoleRepository repo)
    {
        RuleFor(x => x.RoleDTO).SetValidator(new RoleDTOValidator(repo));
    }
}