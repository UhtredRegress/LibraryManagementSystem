using MediatR;

namespace BorrowService.Application.Commands;

public record ReturnBookCommand(int userId, IEnumerable<int> bookIds) : IRequest<bool>;