

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
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker;

var host = Host.CreateDefaultBuilder(args)
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

        services.AddOpenTelemetry("MagicMedia-Worker", (tracing) =>
        {
            //tracing.AddQuartzInstrumentation()
        });

        services.AddSingleton<IUserContextFactory, WorkerUserContextFactory>();
        services.AddMemoryCache();

        //services.AddMassTransitHostedService();
        services.AddHostedService<JobWorker>();
    })
    .Build();

await host.RunAsync();
