using BookService.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;


[AllowAnonymous]
[ApiController]
[Route("query")]
public class QueryController : ControllerBase
{
    private readonly ILogger<QueryController> _logger;
    private readonly IMediator _mediator;

    public QueryController(ILogger<QueryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
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
}