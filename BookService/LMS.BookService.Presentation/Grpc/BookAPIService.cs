using Grpc.Core;
using Lirabry.Grpc;
using LMS.BookService.Domain.Enum;
using LMS.BookService.Infrastructure.Interface;

namespace LMS.BookService.Presentation.Grpc;

public class BookAPIService : BookAPI.BookAPIBase
{
    private readonly ILogger<BookAPIService> _logger;
    private readonly IBookRepository _bookRepository;
    
    public BookAPIService(ILogger<BookAPIService> logger,  IBookRepository bookRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
    }

    public override async Task<BookFoundResponse> FindBook(BookInfoRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received BookInfoRequest in the Book service ");
        var bookFound = await _bookRepository.GetBookByIdAsync(request.BookId);
        var result = new BookFoundResponse();

        if (bookFound == null)
        {
            _logger.LogError("Book not found");
            throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
        }
        result.BookId = bookFound.Id;
        result.Author = bookFound.Author;
        result.Publisher = bookFound.Publisher;
        result.Available = bookFound.Availability == Availability.Available;
        result.Title = bookFound.Title;

        return result;
    }

    public override async Task<CheckExistedBookResponse> CheckExist(CheckExistedBookRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received CheckExistedBookRequest in the Book service ");

        List<int> BookIdList = request.BookId.ToList();

        foreach (var id in BookIdList)
        {
            var result = await _bookRepository.GetBookByIdAsync(id);
            if (result == null || result.Availability != Availability.Available)
            {
                return new CheckExistedBookResponse() {Result = false};
            }
        }
        
        return new CheckExistedBookResponse() {Result = true};
    }
    
}