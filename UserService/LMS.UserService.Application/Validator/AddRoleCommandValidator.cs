using FluentValidation;
using LMS.UserService.Application.Commands;
using LMS.UserService.Infrastructure.Interface;

namespace LMS.BookService.Application.Business.Validator;

public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleCommandValidator(IRoleRepository repo)
    {
        RuleFor(x => x.RoleDTO.Title).NotEmpty().WithMessage("Title is required")
            .MustAsync(async (title, _ ) => await repo.GetRoleByTitleAsync(title) == null)
            .WithMessage("Title is already existed");
    }
}