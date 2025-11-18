using System.Security.Claims;
using BookService.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("ebooks")]
public class EBookReadController : ControllerBase
{
    private readonly ILogger<EBookReadController> _logger;
    private readonly IMediator _mediator;

    public EBookReadController(IMediator mediator, ILogger<EBookReadController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{bookId:int}/read")]
    public async Task<IActionResult> GetBookRead([FromRoute] int bookId)
    {
        _logger.LogInformation("Started read book {bookId}", bookId);
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            var result = await _mediator.Send(new ReadEbookQuery(bookId: bookId,userId: userId));
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully handled read book operation ");   
                return Ok(new { url = result.Value });
            }
            else
            {
                _logger.LogInformation("Failed read book operation due to business error ");
                return BadRequest(new {error = result.Errors.Select(e => e.Message) });        
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("Failed to read book. Please try again later");
        }

    }
}