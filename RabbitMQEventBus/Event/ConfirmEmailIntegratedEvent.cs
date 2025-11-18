namespace RabbitMQEventBus;

public class ConfirmEmailIntegratedEvent : IntegrationEvent
{
    public string Email {get; set;}
    public string Token {get; set;}

    public ConfirmEmailIntegratedEvent(string email, string token)
    {
        Email = email;
        Token = token;
    }
}