namespace RabbitMQEventBus;

public class BorrowHistoryCreatedIntegratedEvent : IntegrationEvent
{
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<int> BookIds { get; set; }

    public BorrowHistoryCreatedIntegratedEvent(string userName, string email, DateTime startDate, DateTime endDate,
        IEnumerable<int> bookIds)
    {
        Username = userName;
        Email = email;
        StartDate = startDate;
        EndDate = endDate;
        BookIds = bookIds;
    }
}