namespace ApiGateway;

public class MetricCollectorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetricService _metricService;
    
    public MetricCollectorMiddleware(RequestDelegate next, IMetricService metricService)
    {
        _next = next;
        _metricService = metricService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var metric = new KeyMetric();
        try
        {
            await _next(context);
        }
        finally
        {
            metric.GenerateKeyMetric(context);
            await _metricService.StoreKeyMetric(metric);
        }
    }
}