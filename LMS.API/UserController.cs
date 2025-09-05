using LMS.Business.Commands;
using LMS.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToList();
            return BadRequest(new {Errors = errors});
        }

        try
        {
            var result = await _mediator.Send(new AddUserCommand() { User = user });
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new {Errors = ex.Errors.Select(x => x.ErrorMessage)});
        }
    }
}