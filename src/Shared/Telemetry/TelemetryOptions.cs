namespace MagicMedia.Telemetry;

public class TelemetryOptions
{
    public string ServiceName { get; init; }

    public string? AzureMonitorConnectionString { get; set; }
}
