using FluentValidation;
using LMS.Business.Commands;
using LMS.Infrastructure.Interface;

namespace LMS.Business;

public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleCommandValidator(IRoleRepository repo)
    {
        RuleFor(x => x.roleDTO).SetValidator(new RoleDTOValidator(repo));
    }
}