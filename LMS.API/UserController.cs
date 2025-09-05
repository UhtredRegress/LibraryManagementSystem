using LMS.Business.Commands;
using LMS.Business.Queries;
using LMS.Domain.Model;
using LMS.Shared.DTOs;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO userLogin)
    {
        try
        {
            var result = await _mediator.Send(new UserLoginQuery(userLogin.Username, userLogin.Password));
            return Ok(new { Token = result });
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(x => x.ErrorMessage) });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Errors = ex.Message });
        }
    }
}