using System.Security.Claims;
using LMS.BorrowService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LMS.BorrowService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BorrowController:ControllerBase
{
    private readonly ILogger<BorrowController> _logger; 
    private readonly IMediator _mediator;

    public BorrowController(ILogger<BorrowController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RequestBorrow(IEnumerable<int> bookIds)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(JwtRegisteredClaimNames.Jti));
        var userName = User.FindFirstValue("name");
        var userAddress = User.FindFirstValue(JwtRegisteredClaimNames.Address);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userPhone = User.FindFirstValue(JwtRegisteredClaimNames.PhoneNumber);
        try
        {
            var result =
                await _mediator.Send(new AddBorrowHistoryCommand(userId, userName, userAddress, userPhone, userEmail, bookIds));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
}