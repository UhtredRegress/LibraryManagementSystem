using LMS.Shared.DTOs;
using LMS.UserService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.UserService.Presentation.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController:ControllerBase
{
    private readonly IMediator _mediator;

    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDTO roleDTO)
    {
        var command = new AddRoleCommand(roleDTO);

        try
        {
            var resultRole = await _mediator.Send(command);
            return Ok(resultRole);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Message); 
        }
    }
}