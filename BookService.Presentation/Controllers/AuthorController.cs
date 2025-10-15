using BookService.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("authors")]
public class AuthorController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(IMediator mediator,  ILogger<AuthorController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewAuthor([FromBody] string Name)
    {
        _logger.LogInformation("Adding ")
    }
}