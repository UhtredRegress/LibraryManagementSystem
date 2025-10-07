namespace ApiGateway;

public class FlushWorker : BackgroundService
{
    private readonly IMetricService _metricService;
    private readonly ILogger<FlushWorker> _logger;

    public FlushWorker(IMetricService metricService,  ILogger<FlushWorker> logger)
    {
        _metricService = metricService;
        _logger = logger;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_metricService.RedisContainsAnyData())
                {
                    _logger.LogInformation("Flushing metrics start");
                    await _metricService.FlushMetrics();
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Flushing metrics failed");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                _logger.LogInformation("Retry after 30 seconds");
            }
        }
    }
}