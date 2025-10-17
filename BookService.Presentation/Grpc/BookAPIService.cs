using BookService.Infrastructure;
using BookService.Infrastructure.Interface;
using Grpc.Core;
using Lirabry.Grpc;


namespace BookService.Presentation.Grpc;

public class BookAPIService : BookAPI.BookAPIBase
{
    private readonly ILogger<BookAPIService> _logger;
    private readonly IBookRepository _bookRepository;
    private readonly IBookPriceRepository _bookPriceRepository;
    
    public BookAPIService(ILogger<BookAPIService> logger,  IBookRepository bookRepository, IBookPriceRepository bookPriceRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
        _bookPriceRepository = bookPriceRepository;
    }

    public override async Task<BookFoundResponse> FindBook(BookInfoRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received BookInfoRequest in the Book service ");
        var bookFound = await _bookRepository.GetBookByIdAsync(request.BookId);
        var result = new BookFoundResponse();

        if (bookFound == null)
        {
            _logger.LogInformation("Book not found");
            throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
        }
        result.BookId = bookFound.Id;
        result.Publisher = bookFound.Publisher;
        result.Title = bookFound.Title;

        return result;
    }

    public override async Task<CheckExistedBookResponse> CheckExist(CheckExistedBookRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received CheckExistedBookRequest in the Book service ");

        List<int> BookIdList = request.BookId.ToList();

        var foundBookList = await _bookRepository.GetRangeBookByIdAsync(BookIdList);

        if (foundBookList.Count() != BookIdList.Count)
        {
            _logger.LogInformation("There are book not found in the database ");
            return new CheckExistedBookResponse() { Result = false };
        }

        foreach (var bookItem in foundBookList)
        {
            if (bookItem.Stock <= 0)
            {
                _logger.LogInformation("Books are not available in the library");
                return new CheckExistedBookResponse() { Result = false };
            }
        }
        
        return new CheckExistedBookResponse() {Result = true};
    }

    public override async Task<GetBookInfoResponse> GetRangeBook(GetBookInfoRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received GetRangeBookRequest in the book service");

        var response = new GetBookInfoResponse();

        foreach (var id in request.BookId)
        {
            var foundBook = await _bookRepository.GetBookByIdAsync(id);
            var responseBook = new Book();
            responseBook.BookId = foundBook.Id;
            responseBook.Title = foundBook.Title;
            responseBook.Publisher = foundBook.Publisher;
            response.Books.Add(responseBook); 
        }

        _logger.LogInformation("Finished processed the Request");
        return response;
    }

    public override async Task<RetrieveBookPriceResponse> RetrieveBookPrice(RetrieveBookPriceRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received RetrieveBookPriceRequest in the Book service ");
        
        var foundBookPrice = await _bookPriceRepository.GetBookPriceFiltered(fb => fb.BookId == request.BookId && (int)fb.BookType == request.BookType);
  
        var response = new RetrieveBookPriceResponse();
        
        if (foundBookPrice == null)
        {
            response.IsSuccess = false;
            return response;
        }
        

        var price = foundBookPrice.PriceUnit;
        var unit = (int)price;
        var micro = (int) ((price - Math.Truncate(price)) * 1000000); 
        var foundBook = await  _bookRepository.GetBookByIdAsync(request.BookId);
        
        response.IsSuccess = true;
        Book temp = new Book();
        temp.BookId = foundBook.Id;
        temp.Title = foundBook.Title;
        temp.Publisher = foundBook.Publisher;
        temp.PricePerUnit = new DecimalValue() {Units = unit, Micros = micro};

        response.Book = temp;

        return response;
    }
    
}