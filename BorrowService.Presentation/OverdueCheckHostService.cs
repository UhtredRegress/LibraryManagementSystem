using BorrowService.Infrastructure.IRepository;
using RabbitMQEventBus;

namespace BorrowService.Presentation;

public class OverdueCheckHostService : BackgroundService
{
    private readonly ILogger<OverdueCheckHostService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBus _eventBus;
    
    private readonly int _runHours = 9;
    
    public OverdueCheckHostService(ILogger<OverdueCheckHostService> logger, IServiceProvider serviceProvider, IEventBus eventBus) 
    {
        _logger = logger;
       _eventBus = eventBus;
       _serviceProvider = serviceProvider;
    } 
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start checking overdue books ");
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, _runHours, 0, 0);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;
            
            await DoWorkAsync(stoppingToken);
            await Task.Delay(delay, stoppingToken);
        }
    }
    

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope(); 
        var borrowHistoryRepository = scope.ServiceProvider.GetRequiredService<IBorrowHistoryRepository>();
        var foundData = await borrowHistoryRepository.GetNearOverdueLoan(2);
        var groups = foundData.GroupBy(x => new {BorrowerId = x.BorrowerId, Username = x.Borrower.Name, Email = x.Borrower.Email}).Select(g => new
        {
            Username = g.Key.Username,
            Email = g.Key.Email,
            BookNearOverdue = g.Select(b => (b.BookId, b.EndDate)).ToList(),
        });
        foreach (var group in groups)
        {
            await _eventBus.PublishAsync(new ReminderOverdueIntegrationEvent() {Email = group.Email, Username = group.Username, BookInfo = group.BookNearOverdue });
        }
    }
}