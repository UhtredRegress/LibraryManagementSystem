using System.Security.Claims;
using BorrowService.Application.Commands;
using BorrowService.Application.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

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
                await _mediator.Send(new AddBorrowHistoryCommand(userIdClaim, userName, userAddress, userPhone,
                    userEmail, days, bookIds));
            if (result.IsSuccess)
            {
                return Ok(new { message = "You have borrowed successfully these book", borrowHistory = result.Value });
            }
            else
            {
                var error = result.Errors.First();
                if (error.Metadata.Count != 0)
                {
                    return BadRequest(new { message = error.Message, data = error.Metadata });
                }
                else
                {
                    return BadRequest(new { message = error.Message });
                }
            }
        }
        catch (ValidationException ex)
        {
            return  BadRequest(new { message = ex.Errors.Select(e => e.ErrorMessage).ToList() });
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
        var username  = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        var address = User.FindFirstValue(JwtRegisteredClaimNames.Address);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var phoneNumber = User.FindFirstValue(JwtRegisteredClaimNames.PhoneNumber);

        try
        {
            var result =
                await _mediator.Send(new ReturnBookCommand(userIdClaim, email, username, address, phoneNumber,
                    bookIds));
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = "You have successfully request to return please wait for response",
                    bookList = result.Value
                });
            }
            else
            {
                var error = result.Errors.First();
                if (error.Metadata.Count != 0)
                {
                    return BadRequest(new { message = error.Message, data = error.Metadata });
                }
                else
                {
                    return BadRequest(new { message = error.Message });
                }
            }
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBorrowHistories(int page = 1, int pageSize = 20)
    {
        try
        {
            var userIdClaim = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _mediator.Send(new MyBorrowHistoriesQuery(userIdClaim, page, pageSize));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "There was a problem with your request");
        }
    }
}