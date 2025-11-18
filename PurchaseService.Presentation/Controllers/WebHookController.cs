using MediatR;
using Microsoft.AspNetCore.Mvc;
using PurchaseService.Application;
using Stripe;
using Stripe.Checkout;

namespace PurchaseService.Presentation.Controllers;

[ApiController]
[Route("webhook")]
public class WebHookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WebHookController> _logger;
    private readonly IMediator _mediator;

    public WebHookController(IConfiguration configuration, ILogger<WebHookController> logger, IMediator mediator)
    {
        _configuration = configuration;
        _logger = logger;
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        var secret = _configuration["Stripe:WebHookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                secret,
                throwOnApiVersionMismatch: false
            );

           
            _logger.LogInformation($"Received event: {stripeEvent.Type}");

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                {
                    var session = stripeEvent.Data.Object as Session;
                    _logger.LogInformation($"Checkout session completed: {session.Id}");
                    try
                    {
                        await _mediator.Send(new SuccessfullyPurchaseBookCommand(session.Metadata["purchase_id"],
                            session.Url));
                        return Ok("Successfully purchased book");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while checking out book: {session.Id}");
                        return BadRequest(ex.Message);
                    }
                }


                case "checkout.session.expired":
                {
                    var session = stripeEvent.Data.Object as Session;
                    _logger.LogInformation($"Checkout session expired: {session.Id}");
                    try
                    {
                        await _mediator.Send(new ExpiredPurchaseBookCommand(session.Metadata["purchase_id"],
                            session.Url));
                        return Ok("Successfully purchased book");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while checking out book: {session.Id}");
                        return BadRequest(ex.Message);
                    }
                }

                default:
                    _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook signature verification failed");
            return BadRequest();
        }
    }
}