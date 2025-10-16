using BookService.Application.Commands;
using FluentValidation;

namespace BookService.Application.Validator;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("Name must contain only alphabet characters");
    }
}