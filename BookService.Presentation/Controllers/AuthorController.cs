using BookService.Application;
using BookService.Application.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/authors")]
public class AuthorController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(IMediator mediator,  ILogger<AuthorController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("add")]
    public async Task<IActionResult> CreateNewAuthor([FromBody] string name)
    {
        _logger.LogInformation("Received request to add new author");
        try
        {
            var result = await _mediator.Send(new CreateAuthorCommand(name));
            if (result.IsSuccess)
            {
                return Ok(new { message = "Successfully created new author", data = result.Value });
            }
            else
            {
                return BadRequest(new {message = result.Errors.Select(m => m.Message)});
            }
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("This request is invalid due to validation");
            return BadRequest(new { error = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was problem with the server please try again later");
        }
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateAuthor([FromRoute]int id, [FromBody] string name)
    {
        _logger.LogInformation("Received request to update author");
        try
        {
            var result = await _mediator.Send(new UpdateAuthorCommand(id, name));
            if (result.IsSuccess)
            {
                return Ok(new { message = $"Successfully update author with id {id}", data = result.Value });
            }
            else
            {
                return BadRequest(new {message = result.Errors.Select(m => m.Message)});
            }
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("This request is invalid due to validation");
            return BadRequest(new { error = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was problem with the server please try again later");
        }
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteAuthor([FromRoute] int id)
    {
        _logger.LogInformation("Received request to delete author");
        try
        {
            var result = await _mediator.Send(new DeleteAuthorCommand(id));
            if (result == true)
            {
                return Ok(new { message = "Successfully deleted author", data = id });
            }
            else
            {
                return BadRequest(new { message = "Deleted failed due to no existence of request id in database" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was problem with the server please try again later");
        }
    }
}