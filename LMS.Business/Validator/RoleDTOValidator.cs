using FluentValidation;
using LMS.Infrastructure.Interface;
using LMS.Shared.DTOs;

namespace LMS.Business.Validator;

public class RoleDTOValidator : AbstractValidator<RoleDTO>
{
    public RoleDTOValidator(IRoleRepository roleRepo)
    {
        RuleFor(role => role.Title)
            .NotEmpty().WithMessage("Title is required")
            .MustAsync(async (title, token) => await roleRepo.GetRoleByTitleAsync(title) == null)
            .WithMessage("Title is already existed");
    }       
}