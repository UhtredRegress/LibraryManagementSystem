using System.Text.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using StackExchange.Redis;

namespace ApiGateway;

public class MetricService : IMetricService
{
    private readonly InfluxDBClient _influxDbClient;
    private readonly IConfiguration _configuration;
    private readonly IDatabase _redis;
    private readonly IServer _server;
    private readonly ILogger<MetricService> _logger;

    public MetricService(IConnectionMultiplexer connection, IConfiguration configuration, ILogger<MetricService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _redis = connection.GetDatabase();
        _server = connection.GetServer("localhost", 6378);
        _influxDbClient = new InfluxDBClient(configuration.GetConnectionString("InfluxDb:Url"),configuration.GetConnectionString("InfluxDb:Token"));
    }
    public async Task StoreKeyMetric(KeyMetric keyMetric)
    {
        var ticks = keyMetric.StartTime.Ticks - (keyMetric.StartTime.Ticks % TimeSpan.FromMinutes(5).Ticks);
        var timeBucket = new DateTime(ticks);
        var key = $"{timeBucket}_{keyMetric.Endpoint}_{keyMetric.StatusCode}";
        var valueMetric = new ApiMetric();
        valueMetric.Count = 1;
        valueMetric.TotalResponseTime = keyMetric.ReponseTimeProccess;
        if (_redis.KeyExists(key))
        {
            try
            {
                var value = JsonSerializer.Deserialize<ApiMetric>(await _redis.StringGetAsync(key));
                valueMetric.Count += value.Count;
                valueMetric.TotalResponseTime += keyMetric.ReponseTimeProccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get response from Redis generate new value for this key");
            }
        }
        await _redis.StringSetAsync(key, JsonSerializer.Serialize(valueMetric));
    }

    public async Task FlushMetrics()
    {
        var keys = _server.Keys(database: 0).Select(k => (string)k).ToList();
        var apiWrite = _influxDbClient.GetWriteApiAsync();
        foreach (var key in keys)
        {
            if (string.IsNullOrEmpty(key))
            {
                await _redis.KeyDeleteAsync(key);
                continue;
            }
            string? json = await _redis.StringGetAsync(key);
            
            try
            {
                var value = JsonSerializer.Deserialize<ApiMetric>(json);
                var keyArray = key.Split('_');
                var status = keyArray[2];
                var path = keyArray[1];
                DateTime.TryParse(keyArray[0], out DateTime time);
                var point = PointData.Measurement("api_metric").Tag("status", status).Tag("path", path).Field("value", value.Count).Field("total_response_time", value.TotalResponseTime.TotalSeconds).Timestamp(time, WritePrecision.S); 
                await apiWrite.WritePointAsync(point, _configuration.GetConnectionString("InfluxDb:Bucket"),_configuration.GetConnectionString("InfluxDb:Org"));
                await _redis.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await _redis.KeyDeleteAsync(key);
            }
        }
    }

    public bool RedisContainsAnyData()
    {
        return _server.DatabaseSize() != 0 ;
    }
}