using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;

namespace BookService.Application;

public class PushEventWorker : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<PushEventWorker> _logger;

    public PushEventWorker(IEventBus eventBus, ILogger<PushEventWorker> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var seenArr = new int[60];
        Array.Fill(seenArr, 5); 
        while (!stoppingToken.IsCancellationRequested)
        {
            int minuteNow = DateTime.UtcNow.Minute;
            seenArr[(minuteNow + 1) % 60] = 5;
            if (seenArr[minuteNow] > 0 && QueueService.Queue.Count != 0)
            {
                var @event = QueueService.Queue.Dequeue();
                _logger.LogInformation("Start send message to the queue");
                await _eventBus.PublishAsync(@event);
                seenArr[minuteNow]--;
            }
            _logger.LogInformation($"{minuteNow} current quota is {seenArr[minuteNow]} ");
            await Task.Delay(5000, stoppingToken);
        }
    }
}