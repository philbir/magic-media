using System.Diagnostics;
using System.Text;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using HotChocolate.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
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
            .AddTelemetrySdk();

        return resourceBuilder;
    }

    public static TelemetryOptions GetTelemetryOptions(this IConfiguration configuration)
    {
        return configuration
           .GetSection("Telemetry")
           .Get<TelemetryOptions>();
    }

    public static IServiceCollection UseOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracerProviderBuilder>? builder = null)
    {
        TelemetryOptions otelOptions = configuration.GetTelemetryOptions();
        ResourceBuilder resourceBuilder = CreateResourceBuilder(otelOptions.ServiceName);

        OpenTelemetryBuilder telemetryBuilder = services.AddOpenTelemetry().WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHotChocolateInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMongoDBInstrumentation()
                .AddSources()
                .AddOtlpExporter(ConfigureOtlp)
                .SetResourceBuilder(resourceBuilder)
                .SetErrorStatusOnException();

            if (Debugger.IsAttached)
            {
                tracing.AddConsoleExporter();
            }

            builder?.Invoke(tracing);
        }).WithMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder);
            metrics.AddHttpClientInstrumentation();
            metrics.AddAspNetCoreInstrumentation();
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddOpenTelemetry(b =>
            {
                b.SetResourceBuilder(resourceBuilder);
                b.AddConsoleExporter();
            });
        });

        if (otelOptions.AzureMonitorConnectionString  is { } connectionString)
        {
            telemetryBuilder.UseAzureMonitor(c =>
            {
                c.ConnectionString = connectionString;
            });
        }

        return services;
    }

    private static void ConfigureOtlp(OtlpExporterOptions o)
    {
        o.ExportProcessorType = ExportProcessorType.Batch;
        o.BatchExportProcessorOptions = new BatchExportActivityProcessorOptions { ExporterTimeoutMilliseconds = 3000 };
        o.Protocol = OtlpExportProtocol.Grpc;
    }

    private static TracerProviderBuilder AddSources(this TracerProviderBuilder tracing)
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


