using System.Runtime.InteropServices.ComTypes;

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
            await Task.Delay(300000, stoppingToken);
            _logger.LogInformation("Flushing metrics start");
            await _metricService.FlushMetrics();
        }
    }
}