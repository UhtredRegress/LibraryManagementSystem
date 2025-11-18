using BookService.Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookService.Application;

public record AuthorBookQuery(int authorId, int page, int pageSize):IRequest<IEnumerable<BookResultDTO>>;

public class AuthorBookQueryHandler : IRequestHandler<AuthorBookQuery, IEnumerable<BookResultDTO>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<AuthorBookQueryHandler> _logger;

    public AuthorBookQueryHandler(IBookRepository bookRepository, ILogger<AuthorBookQueryHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }
    public async Task<IEnumerable<BookResultDTO>> Handle(AuthorBookQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start handler to handle query books by author id {AuthorId}", request.authorId );
        int start = (request.page - 1) * request.pageSize;
        
        _logger.LogInformation("Start retrieve information in database");
        var foundData = await _bookRepository.GetBooksByAuthorIdAsync(request.authorId, start, request.pageSize);
        
        _logger.LogInformation("Mapping result to DTO and return to caller");
        return foundData.Select(x => new BookResultDTO(x.Id, x.Title, x.Authors, x.BookCategories, x.Publisher, x.Type, x.Stock, x.FileAddress)).ToList();
    }
}