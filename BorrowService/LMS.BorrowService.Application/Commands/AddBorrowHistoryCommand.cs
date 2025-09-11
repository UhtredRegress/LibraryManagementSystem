using MediatR;

namespace LMS.BorrowService.Application.Commands;

public record AddBorrowHistoryCommand (int Id, string Name, string Address, string Phone, string Email, int Days , IEnumerable<int> bookList) : IRequest<bool>;