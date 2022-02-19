using Elastic.CommonSchema;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace MagicMedia.Telemetry;

public static class SerilogConfiguration
{

    public static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        //services.AddSingleton<ILoggerFactory>(sp =>
        //{

        //    return loggerFactory;
        //});

        return services;
    }

    public static ILoggingBuilder ConfigureSerilog(this ILoggingBuilder builder, IConfiguration configuration)
    {
        Serilog.Core.Logger? logger = new LoggerConfiguration()
            .ConfigureElastic(configuration)
            .CreateLogger();

        builder.AddSerilog(logger);

        return builder;
    }

    public static LoggerConfiguration ConfigureElastic(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        IServiceProvider? serviceProvider = null)
    {
        TelemetryOptions otelOptions = configuration.GetTelemetryOptions();

        loggerConfiguration.ConfigureEnrich();

        if (otelOptions.ElasticServer is { })
        {
            EcsTextFormatter formatter = CreateFormatter(otelOptions, serviceProvider);
            loggerConfiguration.WriteToElasticsearch(otelOptions, formatter);
        }

        if (true)
        {
            loggerConfiguration.WriteTo.Console();
        }

        return loggerConfiguration;
    }

    private static void ConfigureEnrich(
        this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Enrich.WithSpan();
        loggerConfiguration.Enrich.WithMachineName();
        loggerConfiguration.Enrich.FromLogContext();
    }

    private static void WriteToElasticsearch(
        this LoggerConfiguration loggerConfiguration,
        TelemetryOptions telemetryOptions,
        EcsTextFormatter formatter)
    {
        var elasticsearchSinkOptions = new ElasticsearchSinkOptions(
            new[] { new Uri(telemetryOptions.ElasticServer.Url) })
        {
            CustomFormatter = formatter,
            IndexFormat = $"serilog-logs",
            AutoRegisterTemplate = false,
            BatchAction = ElasticOpType.Create,
            RegisterTemplateFailure = RegisterTemplateRecovery.FailSink,
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
        };

        if (telemetryOptions.ElasticServer?.Username is { })
        {
            elasticsearchSinkOptions.ModifyConnectionSettings = c => c
                    .BasicAuthentication(
                        telemetryOptions.ElasticServer.Username,
                        telemetryOptions.ElasticServer.Password);
        }

        loggerConfiguration.WriteTo.Elasticsearch(elasticsearchSinkOptions);
    }

    private static EcsTextFormatter CreateFormatter(
        TelemetryOptions telemetryOptions,
        IServiceProvider? serviceProvider=null)
    {
        var formatterConfiguration = new EcsTextFormatterConfiguration();

        if ( serviceProvider is { })
        {
            IHttpContextAccessor? httpContextAccessor = serviceProvider
                .GetService<IHttpContextAccessor>();

            if (httpContextAccessor != null)
            {
                formatterConfiguration.MapHttpContext(httpContextAccessor);
            }
        }

        formatterConfiguration.MapCurrentThread(true);
        formatterConfiguration.MapExceptions(true);
        formatterConfiguration.MapCustom((elasticLog, serilogLog) =>
        {
            elasticLog.Event.Dataset = telemetryOptions.ServiceName;
            elasticLog.Event.Module = telemetryOptions.ServiceName;
            elasticLog.Event.Kind = EventKind.Event;
            elasticLog.Event.Outcome = serilogLog.Level < LogEventLevel.Warning
                ? EventOutcome.Success
                : EventOutcome.Failure;

            elasticLog.Labels ??= new Dictionary<string, object>();
            elasticLog.Labels.Add("application.name", telemetryOptions.ServiceName);

            if (serilogLog.TryGetScalarPropertyValue("EventName", out ScalarValue? eventName))
            {
                elasticLog.Event.Provider = (string)eventName.Value;
            }

            return elasticLog;
        });

        return new EcsTextFormatter(formatterConfiguration);
    }
}
