using System;
using MagicMedia;
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
using Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.ConfigureOpenTelemetry();

builder.Services.Configure<HostOptions>(hostOptions =>
{
    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddSingleton((p) =>
{
    FileSystemDiscoveryOptions discoveryOptions = builder.Configuration
        .GetSection("MagicMedia:Discovery")
        .Get<FileSystemDiscoveryOptions>();

    return discoveryOptions;
});
builder.Services
    .AddMagicMediaServer(builder.Configuration)
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
builder.Services.AddSingleton<IGeoDecoderService>(p =>
{
    return new GeoDecoderCacheStore(p.GetRequiredService<MediaStoreContext>(),
        new GoogleMapsGeoDecoderService(p.GetRequiredService<GoogleMapsOptions>()));
});

builder.Services.AddSingleton<IUserContextFactory, WorkerUserContextFactory>();
builder.Services.AddMemoryCache();

builder.Services.AddMassTransitHostedService();
builder.Services.AddHostedService<JobWorker>();

Console.WriteLine($"Applicated Builded! {DateTime.UtcNow}");

var host = builder.Build();
host.Run();
