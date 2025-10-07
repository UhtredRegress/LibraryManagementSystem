namespace ApiGateway;

public class KeyMetric
{
    public string Endpoint { get;  set; }
    public string Method { get; set; }
    public int StatusCode { get; set; }
   
    public TimeSpan ReponseTimeProccess { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public KeyMetric()
    {
        StartTime = DateTime.UtcNow;
    }

    public void GenerateKeyMetric(HttpContext context)
    {
        ReponseTimeProccess = DateTime.UtcNow - StartTime;
        Method = context.Request.Method;
        StatusCode = context.Response.StatusCode;
        Endpoint = context.Request.Path;
    }
}