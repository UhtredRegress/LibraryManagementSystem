using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.NotificationService
{
    public class BorrowHistoryNotificationHandler(IEmailService emailService, ILogger<BorrowHistoryCreatedIntegratedEvent> logger) : IIntegrationEventHandler<BorrowHistoryCreatedIntegratedEvent>
    {
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<BorrowHistoryCreatedIntegratedEvent> _logger = logger;

        public Task Handle(BorrowHistoryCreatedIntegratedEvent @event)
        {
            _logger.LogInformation("NotificationService receive borrow history");
            return Task.CompletedTask; 
        }
    }
}
