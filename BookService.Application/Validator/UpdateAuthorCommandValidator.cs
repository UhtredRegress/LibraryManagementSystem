using BookService.Application.Commands;
using FluentValidation;

namespace BookService.Application.Validator;

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id cannot be empty")
            .Must(i => i > 0).WithMessage("Id must be greater than zero");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("Name must contain only alphabet characters");
    }
}