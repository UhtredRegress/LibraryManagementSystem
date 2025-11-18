using NotificationService.Helper;
using RabbitMQEventBus;

namespace NotificationService;
public class ConfirmEmailIntegratedEventHandler : IIntegrationEventHandler<ConfirmEmailIntegratedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ConfirmEmailIntegratedEventHandler> _logger;
    public ConfirmEmailIntegratedEventHandler(IEmailService emailService, ILogger<ConfirmEmailIntegratedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task Handle(ConfirmEmailIntegratedEvent @event)
    {
        _logger.LogInformation("Confirming email notification starting ");
        await _emailService.SendEmailAsync(@event.Email, "Confirm your email",  CreateMailHelper.CreateMailToNofifyToken(@event.Token));
        _logger.LogInformation("Confirming email notification ending ");
    }
}