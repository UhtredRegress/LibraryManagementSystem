using MediatR;

namespace LMS.BorrowService.Application.Commands;

public record ReturnBookCommand(int userId, IEnumerable<int> bookIds) : IRequest<bool>;