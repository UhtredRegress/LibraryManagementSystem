using RabbitMQEventBus;
using Lirabry.Grpc;
using LMS.NotificationService.Helper;

namespace LMS.NotificationService
{
    public class BorrowHistoryNotificationHandler(IEmailService emailService, ILogger<BorrowHistoryNotificationHandler> logger, BookAPI.BookAPIClient client) : IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>
    {
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<BorrowHistoryNotificationHandler> _logger = logger;
        private readonly BookAPI.BookAPIClient _client = client;

        public async Task Handle(BorrowHistoryCreatedIntegratedEvent @event)
        {
            _logger.LogInformation("NotificationService receive borrow history");
            var requestBookList = new GetBookInfoRequest();
            requestBookList.BookId.Add(@event.BookIds);
            var response = await _client.GetRangeBookAsync(requestBookList);  
            
            var body = CreateMailHelper.CreateMailToNotifyBorrowStatus(username: @event.Username, startDate: @event.StartDate , returnDate: @event.EndDate, response: response);
            _emailService.SendEmailAsync(to: @event.Email, subject:"Borrow books from the Library Management System", body: body);
            _logger.LogInformation("NotificationService finished sent email");
        }
    }
}
