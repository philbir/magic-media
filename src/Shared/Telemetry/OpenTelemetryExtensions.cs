using System.Diagnostics;
using System.Text;
using HotChocolate.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MagicMedia.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var otelConfig = builder.Configuration.GetTelemetryOptions();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(c => c.AddService(otelConfig.ServiceName))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                //metrics.AddRuntimeInstrumentation();
                metrics.AddMeter("magicmedia.core.processing");
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddMongoDBInstrumentation();
                tracing.AddMassTransitInstrumentation();
                tracing.AddSource("MagicMedia*");
            });
        // Use the OTLP exporter if the endpoint is configured.
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        useOtlpExporter = true;
        if (useOtlpExporter)
        {
            //builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }


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


    public static IServiceCollection AddOpenTelemetry_(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracerProviderBuilder>? builder = null)
    {
        TelemetryOptions otelOptions = configuration.GetTelemetryOptions();
        ResourceBuilder resourceBuilder = CreateResourceBuilder(otelOptions.ServiceName);

        services.AddOpenTelemetry().WithTracing(tracing =>
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
        }).WithMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder);
            metrics.AddHttpClientInstrumentation();
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddOtlpExporter(ConfigureOtlp);
//metrics.AddConsoleExporter();

        });

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


