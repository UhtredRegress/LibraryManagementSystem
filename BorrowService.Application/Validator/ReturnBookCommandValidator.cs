using BorrowService.Application.Commands;
using FluentValidation;
using FluentValidation.Validators;

namespace BorrowService.Application.Validator;

public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithMessage("UserId should not be empty");
        RuleFor(x=> x.Name).NotNull().WithMessage("Name cannot be empty");
        RuleFor(x => x.Phone).NotNull().WithMessage("Phone is required");
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required")
            .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email is not valid");
        RuleFor(x => x.Address).NotNull().WithMessage("Address is required");
        RuleFor(x => x.BookList)
            .NotNull().WithMessage("BookList is required")
            .Must(bookList =>
            {
                var bookSet = bookList.ToHashSet();
                return bookSet.Count == bookList.Count();
            }).WithMessage("You request to return duplicate books");
    }
}