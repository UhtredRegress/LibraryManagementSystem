using MediatR;

namespace BorrowService.Application.Commands;

public record AddBorrowHistoryCommand (string userId, string Name, string Address, string Phone, string Email, int Days , IEnumerable<int> bookList) : IRequest<bool>;