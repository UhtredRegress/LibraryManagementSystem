using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Service.Interface;

namespace NotificationService.Controller;

[Authorize]
[ApiController]
[Route("api/subscribe")]
public class SubscribeNotiController : ControllerBase
{
    private readonly ILogger<SubscribeNotiController> _logger;
    private readonly ISubscribeNotiService _subscribeNotiService;
    public SubscribeNotiController(ILogger<SubscribeNotiController> logger, ISubscribeNotiService subscribeNotiService)
    {
        _logger = logger;
        _subscribeNotiService = subscribeNotiService;
    }

    [HttpPost("noti")]
    public async Task<IActionResult> SubscribeNotificationForBookCategory(
        [FromBody] int categoryId)
    {
        try
        {
            _logger.LogInformation("Receive request to get notifcation for category id {id}",
                categoryId);
            var name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                       throw new ArgumentNullException("Claim name is missing");
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                        throw new ArgumentNullException("Claim email is missing");
            var phone = User.Claims.FirstOrDefault(c => c.Type == "phone_number")?.Value ??
                        throw new ArgumentNullException("Claim phone is missing");

            var result = await _subscribeNotiService.SubscribeNotiForCategory(name, email, phone, categoryId);
            return Ok(new { data = result });
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex.ToString());
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(500, "There was an error processing your request");
        }
    }
}