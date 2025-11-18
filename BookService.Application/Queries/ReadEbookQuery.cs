using BookService.Infrastructure;
using BookService.Infrastructure.Interface;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Enum;
using StackExchange.Redis;
namespace BookService.Application;

public record ReadEbookQuery(int userId, int bookId):IRequest<Result<string>>;

public class ReadEbookQueryHandler : IRequestHandler<ReadEbookQuery, Result<string>>
{
    private readonly ILogger<ReadEbookQueryHandler> _logger;
    private readonly IBookRepository _bookRepository;
    private readonly IGrpcClient _grpcClient;
    private readonly IDatabase _db;
    private readonly IMinioService _minioService;
    public ReadEbookQueryHandler(ILogger<ReadEbookQueryHandler> logger, IBookRepository bookRepository, IConnectionMultiplexer connectionMultiplexer, IGrpcClient grpcClient, IMinioService minioService)
    {
        _bookRepository = bookRepository;
        _db = connectionMultiplexer.GetDatabase();
        _logger = logger;
        _grpcClient = grpcClient;
        _minioService = minioService;
    }
    
    public async Task<Result<string>> Handle(ReadEbookQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started to handle read book query by user {UserID} with ebook of book type {BookID}", request.userId, request.bookId);
        string key = request.userId.ToString() + "_" + request.bookId.ToString() + "_" + BookType.Ebook.ToString();
        
        if (await _db.KeyExistsAsync(key) == true)
        {
            _logger.LogInformation("Link has generated for book with id {BookID} for user {UserID} return it to user", request.bookId, request.userId);
            var value = await _db.StringGetAsync(key);
            if (value.IsNull == false && value.HasValue == true)
            {
                return Result.Ok<string>(value);
            } 
        } 
        
        var result = await _grpcClient.CheckIfAlreadyBuyEbook(request.userId, request.bookId);
        
        if (result == false)
        {
            _logger.LogInformation("The user {UserID} has not buy the Ebook of the book {BookId}", request.userId, request.bookId);
            return Result.Fail(new Error($"You haven't buy the Ebook of the book {request.bookId} yet please buy it first"));
        }
        
        _logger.LogInformation("Started retrieve information of the Book with Id {BookId} in database", request.bookId);
        var foundBook = await _bookRepository.GetBookByIdAsync(request.bookId);

        if (foundBook == null || foundBook.FileAddress == null)
        {
            _logger.LogInformation("Database does not contain file address of the book {BookId}", request.bookId);
            return Result.Fail(new Error($"There is error while trying to process please try again later"));
        }

        _logger.LogInformation("Started generate pre-signed url with expiry time is 10 minutes");
        var resultUrl = await _minioService.CreatePreSignedUrlAsync(foundBook.FileAddress);
        
        _logger.LogInformation("Started store url for reused in Redis for valid timespan");
        await _db.StringSetAsync(key,resultUrl, TimeSpan.FromMinutes(10));
        return Result.Ok(resultUrl);
    }
}