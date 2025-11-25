using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using Shared.DTOs;
using Shared.Exception;
using UserService.Application.Queries;

namespace UserService.Presentation.Controllers;

[Authorize(Policy = "AdminRoleRequirement")]
[ApiController]
[Route("api/roles")]
public class RoleController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IMediator mediator, ILogger<RoleController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddRole([FromBody]RoleDTO roleDTO)
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

    [HttpPut("{roleId:int}")]
    public async Task<IActionResult> UpdateRole([FromRoute]int roleId, [FromBody] RoleDTO roleDTO)
    {
        try
        {
            var resultRole = await _mediator.Send(new UpdateRoleCommand(roleId, roleDTO.Title));
            return Ok(resultRole);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { Error = ex.Errors.SelectMany(x => x.ErrorMessage).ToList() });
        }
        catch (NotFoundDataException ex)
        {
            return NotFound("Role not found");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{roleId:int}")]
    public async Task<IActionResult> DeleteRole([FromRoute] int roleId)
    {
        try
        {
            var result = await _mediator.Send(new DeleteRoleCommand(roleId));
            if (result == false)
            {
                return NotFound("Role not found");
            }

            return Ok("Role deleted");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var result = await _mediator.Send(new AllRolesQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { Error = "There is error while processing your request" });
        }
    }
    
}