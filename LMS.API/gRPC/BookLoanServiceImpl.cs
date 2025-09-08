using Grpc.Core;
using Library.Grpc;
using LMS.Business.Commands;
using MediatR;

public class BookLoanServiceImpl : BookLoanService.BookLoanServiceBase
{
    private readonly IMediator _mediator;

    public BookLoanServiceImpl(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<LoanBookResponse> LoanBook(LoanBookRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _mediator.Send(new LoanBookCommand(request.UserId, request.BookId));
            if (result == true)
            {
                return new LoanBookResponse() {Message = "The book has been loaned", Success = true}; 
            }
            return new LoanBookResponse() {Message = "There is error while loaning", Success = false};
        }
        catch (RpcException rpcEx)
        {
            Console.WriteLine(rpcEx.Message);
            return new LoanBookResponse() {Message = "The book could not be loaned", Success = false};
        }
    }
}