using FluentValidation;
using LMS.BorrowService.Application.Commands;
using LMS.BorrowService.Infrastructure.IRepository;

namespace LMS.BorrowService.Application.Validator;

public class ReturnBookCommandValidator: AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator(IBorrowHistoryRepository borrowHistoryRepository)
    {
        RuleFor(x => x.userId).NotNull().WithMessage("UserId should not be null");
        RuleFor(x => x.bookIds).NotNull().WithMessage("BookIds should not be null")
            .MustAsync(async (x, token) =>
            {
                
            });
    }
}