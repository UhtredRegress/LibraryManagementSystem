using System.Diagnostics.Metrics;

namespace NotificationService.Metric;

public class WebSocketMetric
{
    private readonly Counter<int> _counterWsConnections;
    private readonly UpDownCounter<int> _counterWsActiveConnections;

    public WebSocketMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("LMS.WS");
        _counterWsConnections = meter.CreateCounter<int>("lms.ws.counter_connections");
        _counterWsActiveConnections = meter.CreateUpDownCounter<int>("lms.ws.active_connections");
    }

    public void ConnectionMade(int quantity)
    {
        _counterWsConnections.Add(quantity);
    }

    public void ActiveConnectionFluctuate(int quantity)
    {
        _counterWsActiveConnections.Add(quantity);
    }
}