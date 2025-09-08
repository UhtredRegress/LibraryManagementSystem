using FluentValidation;
using LMS.Business.Commands;
using LMS.Domain.Enum;
using LMS.Infrastructure.Interface;

namespace LMS.Business.Validator;

public class LoanBookCommandValidator : AbstractValidator<LoanBookCommand>
{
    public LoanBookCommandValidator(IUserRepository userRepository, IBookRepository bookRepository)
    {
        RuleFor(x => x.bookId)
            .GreaterThan(0).WithMessage("Book ID must be greater than zero")
            .MustAsync(async (bookId, _) =>
            {
                bool check = true;
                var tempBook = await bookRepository.GetBookByIdAsync(bookId);
                if (tempBook == null)
                {
                    check = false;
                }
                
                if (tempBook.Availability != Availability.Available)
                {
                    check = false;
                } 
                
                return check;
                
            }).WithMessage("Book ID is not valid");
        RuleFor(x => x.userId)
            .GreaterThan(0).WithMessage("User ID must be greater than zero")
            .MustAsync(async (userId, _) => await userRepository.GetUserByIdAsync(userId) != null).WithMessage("User is not found");
    }
}