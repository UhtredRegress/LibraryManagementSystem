namespace RabbitMQEventBus;

public class RequestReturnBookIntegratedEvent : IntegrationEvent
{
    public string Username { get; }
    public string Email { get; } 
    public string Phone { get; } 
    public IEnumerable<int> BookList { get; }

    public RequestReturnBookIntegratedEvent(string username, string email, string phone, IEnumerable<int> bookList)
    {
        Username = username;
        Email = email;
        Phone = phone;
        BookList = bookList;
    }
}