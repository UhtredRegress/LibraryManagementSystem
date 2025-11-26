using BookService.Application;
using BookService.Application.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;


[AllowAnonymous]
[ApiController]
[Route("api/query")]
public class QueryController : ControllerBase
{
    private readonly ILogger<QueryController> _logger;
    private readonly IMediator _mediator;
    private readonly IBookService _bookService;

    public QueryController(ILogger<QueryController> logger, IMediator mediator, IBookService bookService)
    {
        _logger = logger;
        _mediator = mediator;
        _bookService = bookService;
    }

    [HttpGet("authors/{authorId:int}/books")]
    public async Task<IActionResult> GetBooksAsync(int authorId, int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("Receive request to retrieve books by the author id {AuthorId}", authorId);
        try
        {
            var result = await _mediator.Send(new AuthorBookQuery(authorId, page, pageSize));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was an error processing the request please try again later");
        }
    }
    
    [HttpGet("books")]
    public async Task<IActionResult> GetPaginatedBooks([FromQuery]int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? type = null, [FromQuery] ICollection<int>? authorsId = null,
        [FromQuery]ICollection<int>? categoriesId = null, [FromQuery] int? yearPublishedStart = null,[FromQuery] int? yearPublishedEnd = null)
    {
        try
        {
            var result = await _bookService.GetBooksAsync(page, pageSize, type,  authorsId, categoriesId, yearPublishedStart, yearPublishedEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}