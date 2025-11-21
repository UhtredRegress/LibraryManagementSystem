using Microsoft.EntityFrameworkCore;
using NotificationService.DbContext;
using NotificationService.Metric;

namespace NotificationService.Infrastructure.Hub;


public class NotiBookHub:Microsoft.AspNetCore.SignalR.Hub
{
    private readonly NotiDbContext _dbContext;
    private readonly ILogger<NotiBookHub> _logger;
    private readonly WebSocketMetric _webSocketMetric;
    
    public NotiBookHub(NotiDbContext dbContext,  ILogger<NotiBookHub> logger, WebSocketMetric webSocketMetric)
    {
        _dbContext = dbContext;
        _logger = logger; 
        _webSocketMetric = webSocketMetric;
    }

    private async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {

        _logger.LogInformation("OnConnectedAsync received, check whether user is logged in");
        var user = Context.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        if (string.IsNullOrEmpty(user))
        {
            _logger.LogInformation("User is not logged in short circut this");
            return; 
        }
        
        _logger.LogInformation("Check username {username} is existed in notification database", user);
        var foundUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == user);
        if (foundUser == null)
        {
            _logger.LogInformation("User haven't subscription for notification short circuit this");
            return;
        }
        
        _logger.LogInformation("Check user subscription information add add to the hub");
        var notiSubscribe =  await _dbContext.NotiSubscriptions
            .AsNoTracking()
            .Where(no => no.UserId == foundUser.Id)
            .Include(no => no.BookCategory)
            .ToListAsync();

        foreach (var noti in notiSubscribe)
        {
            _logger.LogInformation("Start add connection id {connectionId} to the group name {groupName}", Context.ConnectionId, noti.BookCategory.Name);
            await AddToGroup(noti.BookCategory.Name); 
        }
        _webSocketMetric.ConnectionMade(1);
        _webSocketMetric.ActiveConnectionFluctuate(1);
        _logger.LogInformation("Done operation on connected event");
    }

    public override async Task OnDisconnectedAsync(System.Exception exception)
    {
        _webSocketMetric.ActiveConnectionFluctuate(-1);
    }
    
    
}