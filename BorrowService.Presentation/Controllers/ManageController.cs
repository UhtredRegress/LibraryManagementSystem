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
            if (result.IsSuccess == true)
            {
                return Ok(new { message = "Successfully confirmed book returned", borrowHistory = result.Value });
            }
            else
            {
                var error =  result.Errors.First();
                return BadRequest(new { message = error.Message });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new  { message = ex.Message });
        }
    }
}