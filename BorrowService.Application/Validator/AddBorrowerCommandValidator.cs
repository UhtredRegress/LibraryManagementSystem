using BorrowService.Application.Commands;
using FluentValidation;
using FluentValidation.Validators;

namespace BorrowService.Application.Validator;

public class AddBorrowHistoryCommandValidator:AbstractValidator<AddBorrowHistoryCommand>
{
    public AddBorrowHistoryCommandValidator()
    {
        RuleFor(x => x.userId).NotNull().WithMessage("UserId should not be empty");
        RuleFor(x=> x.Name).NotNull().WithMessage("Name cannot be empty");
        RuleFor(x => x.Phone).NotNull().WithMessage("Phone is required");
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required")
            .EmailAddress(EmailValidationMode.Net4xRegex).WithMessage("Email is not valid");
        RuleFor(x => x.Address).NotNull().WithMessage("Address is required");
        RuleFor(x => x.bookList)
            .NotNull().WithMessage("BookList is required")
            .Must(bookList =>
            {
                var bookListSet = bookList.ToHashSet();
                if (bookListSet.Count != bookList.Count())
                {
                    return false; 
                }

                return true;
            }).WithMessage("You request to borrow duplicate book");
        RuleFor(x=> x.Days).NotNull().WithMessage("Day to returned book is required").Must(x => x > 0).WithMessage("Day to returned book must be positive");
    }
}