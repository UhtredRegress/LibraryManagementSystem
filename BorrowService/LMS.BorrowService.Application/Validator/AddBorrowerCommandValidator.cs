using FluentValidation;
using FluentValidation.Validators;
using LMS.BorrowService.Application.Commands;

namespace LMS.BorrowService.Application.Validator;

public class AddBorrowHistoryCommandValidator:AbstractValidator<AddBorrowHistoryCommand>
{
    public AddBorrowHistoryCommandValidator()
    {
        RuleFor(x=> x.Name).NotEmpty().WithMessage("Name cannot be empty");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email is not valid");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        RuleFor(x=> x.bookList)
            .NotEmpty().WithMessage("BookList is required");
        RuleFor(x=> x.Days).NotEmpty().WithMessage("Day to returned book is required").Must(x => x > 0).WithMessage("Day to returned book must be positive");
    }
}