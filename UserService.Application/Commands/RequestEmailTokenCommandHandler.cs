using MediatR;
using RabbitMQEventBus;
using UserService.Infrastructure.Service.Interface;

namespace UserService.Application.Commands;

public record RequestEmailTokenCommand(string Email, string Status) : IRequest<bool>;

public class RequestEmailTokenCommandHandler : IRequestHandler<RequestEmailTokenCommand, bool>
{
    private readonly IEmailTokenService _emailTokenService;
    private readonly IEventBus _eventBus;
    
    public RequestEmailTokenCommandHandler(IEmailTokenService emailTokenService, IEventBus eventBus)
    {
        _emailTokenService = emailTokenService;
        _eventBus = eventBus;
    }
    
    public async Task<bool> Handle(RequestEmailTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.Status != "Inactive")
        {
            throw new InvalidOperationException("The account is already active");
        }
        
        var token = await _emailTokenService.GenerateToken(request.Email, 60);
        if (token == null)
        {
            return false;
        }
        
        await _eventBus.PublishAsync(new ConfirmEmailIntegratedEvent(request.Email, token));
        return true; 
    }
}