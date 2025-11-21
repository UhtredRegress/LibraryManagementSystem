using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.DbContext;
using NotificationService.Helper;
using NotificationService.Infrastructure.Hub;
using NotificationService.Models;
using RabbitMQEventBus;

namespace NotificationService.EventHandler;

public class NewBookAddedEventHandler : IIntegrationEventHandler<NewBookCreatedIntegratedEvent>
{
    private readonly ILogger<NewBookAddedEventHandler> _logger;
    private readonly NotiDbContext _context;
    private readonly IHubContext<NotiBookHub> _hubContext;

    public NewBookAddedEventHandler(ILogger<NewBookAddedEventHandler> logger, NotiDbContext context, IHubContext<NotiBookHub> hubContext)
    {
        _logger = logger;
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task Handle(NewBookCreatedIntegratedEvent @event)
    {
        try
        {
            _logger.LogInformation("Start handler for NewBookCreatedIntegratedEvent");
            _logger.LogInformation("Create notification and persist in database");
            var noti = new Notification(CreateNotiHelper.CreateNotiBody(@event.Title, @event.Author, @event.Category));

            var notiPersist = await _context.Notifications.AddAsync(noti);
            await _context.SaveChangesAsync();
            noti = notiPersist.Entity;

            _logger.LogInformation("Find cate has been store in the database");
            var cates = @event.Category.Split(", ");

            var foundCate = await _context.BookCategories.Where(c => cates.Contains(c.Name)).AsNoTracking()
                .ToListAsync();

            var userHasSubscribe = await _context.NotiSubscriptions
                .Where(sub => foundCate.Contains(sub.BookCategory))
                .Select(sub => sub.User)
                .Distinct()
                .ToListAsync();
            foreach (var user in userHasSubscribe)
            {
                await _context.NotificationStatuses.AddAsync(new NotificationStatus(user, noti));
            }
            await _context.SaveChangesAsync();

            foreach (var cate in foundCate)
            {
                await _hubContext.Clients.Groups(cate.Name)
                    .SendAsync("ReceiveNotification", CreateNotiHelper.CreateNotiSubject(cate.Name));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in handler for NewBookCreatedIntegratedEvent"); 
        }
    }
}