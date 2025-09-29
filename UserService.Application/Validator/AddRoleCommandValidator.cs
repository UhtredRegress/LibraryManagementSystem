using FluentValidation;
using UserService.Application.Commands;
using UserService.Infrastructure.Interface;

namespace UserService.Application.Business.Validator;

public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleCommandValidator(IRoleRepository repo)
    {
        RuleFor(x => x.RoleDTO.Title).NotEmpty().WithMessage("Title is required")
            .MustAsync(async (title, _ ) => await repo.GetRoleByTitleAsync(title) == null)
            .WithMessage("Title is already existed");
    }
}