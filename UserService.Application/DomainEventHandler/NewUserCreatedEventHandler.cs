using Domain.DomainEvent;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using UserService.Infrastructure.Service.Interface;

namespace UserService.Application.DomainEventHandler;

public class ConfirmEmailHandler :  INotificationHandler<NewUserCreatedEvent>
{
    private readonly IEmailTokenService _emailTokenService;
    private readonly ILogger<ConfirmEmailHandler> _logger;
    private readonly IEventBus _eventBus;

    public ConfirmEmailHandler(IEmailTokenService emailTokenService, ILogger<ConfirmEmailHandler> logger, IEventBus eventBus)
    {
        _emailTokenService = emailTokenService;
        _logger = logger;
        _eventBus = eventBus;
    }
    
    public async Task Handle(NewUserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("NewUserCreatedEvent received");
        var token = await _emailTokenService.GenerateToken(notification.email, 60);
        if (token == null)
        {
            throw new Exception("Invalid email token");
        }
        await _eventBus.PublishAsync(new ConfirmEmailIntegratedEvent(notification.email, token)); 
    }
    
}