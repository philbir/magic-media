namespace MagicMedia.Telemetry;

public class TelemetryOptions
{
    public string ServiceName { get; set; }

    public ElasticServerOptions ElasticServer { get; set; }
}


public class ElasticServerOptions
{
    public string Url { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}
