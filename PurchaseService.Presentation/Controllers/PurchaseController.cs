using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using PurchaseService.Application;

namespace PurchaseService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("purchases")]
public class PurchaseController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PurchaseBook(int bookId, int bookType, int amount)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        
        int.TryParse(userIdClaim, out var userId);

        try
        {
            var result = await _mediator.Send(new PurchaseBookCommand(userId, userName, bookId, bookType, amount));
            return Ok(new { Url = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}