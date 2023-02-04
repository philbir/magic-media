using System.Diagnostics;
using System.Text;
using HotChocolate.Diagnostics;
using Microsoft.Extensions.Configuration;
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

    public static TelemetryOptions GetTelemetryOptions(this IConfiguration configuration)
    {
        return configuration
           .GetSection("Telemetry")
           .Get<TelemetryOptions>();
    }

    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracerProviderBuilder>? builder = null)
    {
        services.ConfigureLogging();

        TelemetryOptions otelOptions = configuration.GetTelemetryOptions();
        ResourceBuilder resourceBuilder = CreateResourceBuilder(otelOptions.ServiceName);

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

            if (Debugger.IsAttached)
            {
                //tracing.AddConsoleExporter();
            }

            builder?.Invoke(tracing);
        });

        services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder);
            metrics.AddHttpClientInstrumentation();
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddOtlpExporter(ConfigureOtlp);

            if (Debugger.IsAttached)
            {
                metrics.AddConsoleExporter();
            }
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


