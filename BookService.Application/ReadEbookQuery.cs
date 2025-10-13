using BookService.Infrastructure.Interface;
using MediatR;
using StackExchange.Redis;
namespace BookService.Application;

public record ReadEbookQuery(int userId, int bookId):IRequest<string>;

public class ReadEbookQueryHandler : IRequestHandler<ReadEbookQuery, string>
{
    private readonly IBookRepository _bookRepository;
    private IDatabase _db;

    public ReadEbookQueryHandler(IBookRepository bookRepository, IConnectionMultiplexer connectionMultiplexer)
    {
        _bookRepository = bookRepository;
        _db = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<string> Handle(ReadEbookQuery request, CancellationToken cancellationToken)
    {
        
    }
}