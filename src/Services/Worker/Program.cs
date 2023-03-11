

using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.GoogleMaps;
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
using Serilog;
using Worker;

Microsoft.Extensions.Hosting.IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json");
        builder.AddUserSecrets<Program>(optional: true);
        builder.AddJsonFile("appsettings.local.json", optional: true);
        builder.AddEnvironmentVariables();
    })
    .UseSerilog((context, provider, loggerConfiguration) =>
    {
        //loggerConfiguration.ConfigureElastic(context.Configuration, provider);
    })
    .ConfigureServices((hostContext, services) =>
    {
        /*
        services.AddOpenTelemetry(hostContext.Configuration, tracing =>
        {
            //tracing.AddQuartzInstrumentation()
        });*/

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
            .AddGoogleMaps()
            .AddAzureAI()
            .AddMongoDbStore()
            .AddFileSystemStore()
            .AddFileSystemDiscovery()
            .AddWorkerMessaging()
            .AddClientThumbprintServices()
            .AddScheduler()
            .AddJobs();

        //TODO: Switch to decorater pattern
        services.AddSingleton<IGeoDecoderService>(p =>
        {
            return new GeoDecoderCacheStore(p.GetRequiredService<MediaStoreContext>(),
                new GoogleMapsGeoDecoderService(p.GetRequiredService<GoogleMapsOptions>()));
        });

        services.AddSingleton<IUserContextFactory, WorkerUserContextFactory>();
        services.AddMemoryCache();

        services.AddMassTransitHostedService();
        services.AddHostedService<JobWorker>();
    })
    .Build();

await host.RunAsync();
