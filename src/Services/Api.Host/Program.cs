using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MagicMedia.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerConfiguration logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", "Api")
                .WriteTo.Console();

            var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
            if (seqUrl != null)
            {
                logConfig.WriteTo.Seq(seqUrl);
            }

            Log.Logger = logConfig.CreateLogger();
            try
            {
                Log.Information("Starting Api");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "API Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(builder =>
               {
                   builder.AddJsonFile("appsettings.json");
                   builder.AddUserSecrets<Program>(optional: true);
                   builder.AddJsonFile("appsettings.local.json", optional: true);
                   builder.AddEnvironmentVariables();
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
