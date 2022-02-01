using System;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Jobs;
using MagicMedia.Messaging;
using MagicMedia.Scheduling;
using MagicMedia.Security;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MagicMedia.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Worker;
public class Program
{
    public static void Main(string[] args)
    {
        //LoggingConfig.Configure("Worker");

        try
        {
            //Log.Information("Starting Worker");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            //Log.Fatal(ex, "Worker terminated unexpectedly");
        }
        finally
        {
            //Log.CloseAndFlush();
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
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<HostOptions>(hostOptions =>
                {
                    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                });

                FileSystemDiscoveryOptions discoveryOptions = hostContext.Configuration
                    .GetSection("MagicMedia:Discovery")
                    .Get<FileSystemDiscoveryOptions>();

                services.AddSingleton(discoveryOptions);

                services
                    .AddMagicMediaServer(hostContext.Configuration)
                    .AddProcessingMediaServices()
                    .AddBingMaps()
                    .AddAzureAI()
                    .AddMongoDbStore()
                    .AddFileSystemStore()
                    .AddFileSystemDiscovery()
                    .AddWorkerMessaging()
                    .AddClientThumbprintServices()
                    .AddScheduler()
                    .AddJobs();

                services.AddOpenTelemetry("MagicMedia-Worker");

                services.AddSingleton<IUserContextFactory, WorkerUserContextFactory>();
                services.AddMemoryCache();

                    //services.AddMassTransitHostedService();
                    services.AddHostedService<JobWorker>();
            });
}
