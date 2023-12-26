using System;
using System.Xml.Schema;
using Azure.Monitor.OpenTelemetry.AspNetCore;
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
using Worker;
using IHost = Microsoft.Extensions.Hosting.IHost;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Console.WriteLine("Building worker...");
builder.Services.UseOpenTelemetry(builder.Configuration);

builder.Services.Configure<HostOptions>(hostOptions =>
{
    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

FileSystemDiscoveryOptions discoveryOptions = builder.Configuration
    .GetSection("MagicMedia:Discovery")
    .Get<FileSystemDiscoveryOptions>();

builder.Services.AddSingleton(discoveryOptions);
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

IHost host = builder.Build();

Console.WriteLine("Starting worker...");
await host.RunAsync();
