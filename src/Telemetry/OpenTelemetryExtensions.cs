using System.Diagnostics;
using System.Text;
using HotChocolate.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MagicMedia.Telemetry;

public static class OpenTelemetryExtensions
{
    private static Uri ExporterEndpoint = new Uri("http://localhost:55680");

    public static ResourceBuilder CreateResourceBuilder(string name)
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateEmpty()
            .AddService(name)
            .AddAttributes(new KeyValuePair<string, object>[]
            {
                new("deployment.environment", "dev")
            })
            .AddTelemetrySdk();

        return resourceBuilder;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string serviceName)
    {
        //services.AddSingleton<ActivityEnricher, CustomActivityEnricher>();
        ResourceBuilder resourceBuilder = CreateResourceBuilder(serviceName);

        services.AddOpenTelemetryTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHotChocolateInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMongoDBInstrumentation()
                .AddMassTransitInstrumentation()
                .AddSources()
                .AddOtlpExporter(ConfigureOtlp)
                .SetResourceBuilder(resourceBuilder)
                    .SetErrorStatusOnException();
        });

        services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder);
            metrics.AddHttpClientInstrumentation();
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddOtlpExporter(ConfigureOtlp);
        });

        return services;
    }

    private static void ConfigureOtlp(OtlpExporterOptions o)
    {
        o.ExportProcessorType = ExportProcessorType.Batch;
        o.MetricReaderType = MetricReaderType.Periodic;
        o.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
        {
            ExportIntervalMilliseconds = 30000
        };

        o.Endpoint = ExporterEndpoint;
        //o.Headers = $"Authorization=Bearer {options.SecretToken}";
        o.Protocol = OtlpExportProtocol.Grpc;
    }

    public static TracerProviderBuilder AddSources(this TracerProviderBuilder tracing)
    {
        tracing.AddSource("MagicMedia*");

        return tracing;
    }
}

public class CustomActivityEnricher : ActivityEnricher
{
    public CustomActivityEnricher(
        ObjectPool<StringBuilder> stringBuilderPool,
        InstrumentationOptions options)
        : base(stringBuilderPool, options)
    {
    }

    protected override string CreateRootActivityName(
        Activity activity,
        Activity root,
        string operationDisplayName)
    {
        return operationDisplayName;
    }
}


