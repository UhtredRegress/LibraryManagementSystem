using System.Security.Claims;
using BorrowService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Exception;

namespace BorrowService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/borrows")]
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
                return BadRequest( new { message = "You request a book that is not available in the library"});
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("return")]
    public async Task<IActionResult> ReturnBook(IEnumerable<int> bookIds)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username  = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        var address = User.FindFirstValue(JwtRegisteredClaimNames.Address);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var phoneNumber = User.FindFirstValue(JwtRegisteredClaimNames.PhoneNumber);

        try
        {
            var result =
                await _mediator.Send(new ReturnBookCommand(userIdClaim, email, username, address, phoneNumber,
                    bookIds));
            return Ok( new {message = "You have successfully request to return please wait for response", bookList = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotFoundDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}