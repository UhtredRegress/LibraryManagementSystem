using System.Security.Claims;
using BorrowService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BorrowService.Presentation.Controllers;

[Authorize(Policy = "LibrarianRoleRequirement")]
[ApiController]
[Route("api/managements")]
public class ManageController : ControllerBase
{
    private readonly IMediator _mediator;

    public ManageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{borrowHistoryId:int}")]
    public async Task<IActionResult> ConfirmBookReturned(int borrowHistoryId)
    {
        int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            var result = await _mediator.Send(new ConfirmBookReturnedCommand(BorrowHistoryId: borrowHistoryId, UserId: userId ));
            return Ok(new { message = "Successfully confirmed book returned", borrowHistory = result });
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new  { message = ex.Message });
        }
    }
}