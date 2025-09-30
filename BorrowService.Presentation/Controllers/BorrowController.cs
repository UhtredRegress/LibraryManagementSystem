using System.Security.Claims;
using BorrowService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BorrowService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/borrow")]
public class BorrowController:ControllerBase
{
    private readonly ILogger<BorrowController> _logger; 
    private readonly IMediator _mediator;

    public BorrowController(ILogger<BorrowController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("borrow")]
    public async Task<IActionResult> RequestBorrow(IEnumerable<int> bookIds, int days) 
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        var userAddress = User.FindFirstValue(JwtRegisteredClaimNames.Address);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userPhone = User.FindFirstValue(JwtRegisteredClaimNames.PhoneNumber);
        
        try
        {
            var result =
                await _mediator.Send(new AddBorrowHistoryCommand(userIdClaim, userName, userAddress, userPhone, userEmail, days, bookIds));
            if (result == false)
            {
                return BadRequest("You request a book that is not existed in the library");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("return")]
    public async Task<IActionResult> ReturnBook(IEnumerable<int> bookIds)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdClaim, out var userId);
        var username  = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        var userAddress = User.FindFirstValue(JwtRegisteredClaimNames.Address); 
        
        return Ok();
    }
}