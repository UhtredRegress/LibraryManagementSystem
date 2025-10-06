using System.Text.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using StackExchange.Redis;

namespace ApiGateway;

public class MetricService : IMetricService
{
    private InfluxDBClient _influxDbClient;
    private readonly IConfiguration _configuration;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _redis;
    private readonly IServer _server;
    private Queue<DateTime> timeBucket = new Queue<DateTime>() {};

    public MetricService(IConnectionMultiplexer connection, IConfiguration configuration)
    {
        _redis = connection.GetDatabase();
        _server = connection.GetServer("localhost", 6378);
        timeBucket.Enqueue(DateTime.UtcNow);
        _configuration = configuration;
        _influxDbClient = new InfluxDBClient(configuration.GetConnectionString("InfluxDb:Url"),configuration.GetConnectionString("InfluxDb:Token"));
    }
    public async Task StoreKeyMetric(KeyMetric keyMetric)
    {
        var key = $"metric_{keyMetric.Endpoint}_{keyMetric.StatusCode}_{timeBucket.Peek()}";
        var valueMetric = new ApiMetric();
        valueMetric.Count = 1;
        valueMetric.TotalResponseTime = keyMetric.ReponseTimeProccess;
        if (_redis.KeyExists(key))
        {
            var value = JsonSerializer.Deserialize<ApiMetric>(await _redis.StringGetAsync(key));
            valueMetric.Count += value.Count;
            valueMetric.TotalResponseTime += keyMetric.ReponseTimeProccess;
        }
        
        await _redis.StringSetAsync(key, JsonSerializer.Serialize(valueMetric));
    }

    public async Task FlushMetrics()
    {
        timeBucket.Enqueue(DateTime.UtcNow);
        timeBucket.Dequeue();
        var keys = _server.Keys(database: 0).Select(k => (string)k).ToList();
        var apiWrite = _influxDbClient.GetWriteApiAsync();
        foreach (var key in keys)
        {
            var value = JsonSerializer.Deserialize<ApiMetric>(await _redis.StringGetAsync(key));
            var keyArray = key.Split('_');
            var status = keyArray[2];
            var path = keyArray[1];
            DateTime.TryParse(keyArray[3], out DateTime time);
            var point = PointData.Measurement("api_metric").Tag("status", status).Tag("path", path).Field("value", value.Count).Field("total_response_time", value.TotalResponseTime).Timestamp(time, WritePrecision.S); 
            await apiWrite.WritePointAsync(point, _configuration.GetConnectionString("InfluxDb:Bucket"),_configuration.GetConnectionString("InfluxDb:Org"));
            await _redis.KeyDeleteAsync(key);
        }
    }
}