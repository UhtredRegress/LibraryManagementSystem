using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using UserService.Application.Commands;
using UserService.Application.Queries;
using UserService.Domain.Model;
using Shared.DTOs;

namespace UserService.Presentation.Controllers;


[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize(Policy = "AdminRoleRequirement")]
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
    
    [Authorize(Policy = "AdminRoleRequirement")]
    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var result = await _mediator.Send(new DeleteUserCommand(Id: userId));
            if (result == true)
            {
                return Ok("User successfully deleted");
            }
            else
            {
                return BadRequest("User not found");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { Errors = ex.Message });
        }
    }
    
    [AllowAnonymous]
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
    
    [Authorize]
    [HttpPost("email-confirm")]
    public async Task<IActionResult> ConfirmEmail([FromBody]string code)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var statusClaim = User.FindFirst("status")?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        try
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(idClaim, emailClaim, statusClaim, code));
            if (result == false)
            {
                return BadRequest("Your code is not correct");
            }

            return Ok("Your account is successfully confirmed ");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Errors = ex.Message });
        }
    }
    
    [Authorize]
    [HttpGet("request-confirm")]
    public async Task<IActionResult> RequestConfirm()
    {
        var statusClaim = User.FindFirst("status")?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        try
        {
            var result = await _mediator.Send(new RequestEmailTokenCommand(emailClaim, statusClaim));
            if (result == false)
            {
                return BadRequest("The token can not be generated");
            }

            return Ok("The token has been generated");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Errors = ex.Message });
        }
    }
}