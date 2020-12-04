using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace MagicMedia
{
    public static class LoggingConfig
    {
        public static void Configure(string serviceName)
        {
            LoggerConfiguration logConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", serviceName)
                .WriteTo.Console();

            var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
            if (seqUrl != null)
            {
                logConfig.WriteTo.Seq(seqUrl);
            }

            var esUrl = Environment.GetEnvironmentVariable("ES_URL");
            if (esUrl != null)
            {
                logConfig.Enrich.WithElasticApmCorrelationInfo()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUrl))
                {
                    CustomFormatter = new EcsTextFormatter()
                });
            };

            Log.Logger = logConfig.CreateLogger();
        }

    }
}
