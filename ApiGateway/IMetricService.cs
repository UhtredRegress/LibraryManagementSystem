namespace ApiGateway;

public interface IMetricService
{
    Task StoreKeyMetric(KeyMetric keyMetric); 
    Task FlushMetrics();
    bool RedisContainsAnyData();
}