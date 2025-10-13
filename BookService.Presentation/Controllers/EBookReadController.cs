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
    private readonly IMediator _mediator;

    public EBookReadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{bookId:int}/read")]
    public async Task<IActionResult> GetBookRead([FromRoute] int bookId)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _mediator.Send(new ReadEbookQuery(bookId, userId));
        
        throw new NotImplementedException();
    }
}