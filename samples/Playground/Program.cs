using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Messaging;
using MagicMedia.Playground;
using MagicMedia.Security;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MagicMedia.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;

namespace Playground
{
    class Program
    {
        private static readonly ActivitySource PlaygroundActivitySource = new ActivitySource("MagicMedia.Playground");

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            IServiceProvider sp = BuildServiceProvider();
            BulkMediaUpdater updater = sp.GetService<BulkMediaUpdater>();
            VideoConverter videoConverter = sp.GetService<VideoConverter>();
            FaceScanner faceScanner = sp.GetService<FaceScanner>();
            ImageHasher hasher = sp.GetService<ImageHasher>();
            ClientThumbprintLoader thumbprintLoader = sp.GetService<ClientThumbprintLoader>();

            //FileSystemSnapshotBuilder.BuildSnapshot();
            //await videoConverter.GenerateVideosAsync(default);

            //await updater.UpdateMediaAISummaryAsync(default);

            //await hasher.GetDuplicatesAsync();
            //await hasher.HashAsync();

            await updater.UpdateLocationAsync(CancellationToken.None);
            //await faceScanner.RunAsync(default);

            //await updater.DeleteMediaAIOrphansAsync();

            //await thumbprintLoader.LoadAuditThumbprintsAsync();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile("appsettings.local.json", optional: true)
                 .AddUserSecrets<Program>(optional: true)
                 .Build();

            var services = new ServiceCollection();
            services
                .AddMagicMediaServer(config)
                .AddProcessingMediaServices()
                .AddBingMaps()
                .AddMongoDbStore()
                .AddFileSystemStore()
                .AddFileSystemDiscovery()
                .AddClientThumbprintServices()
                .AddWorkerMessaging();

            services.AddSingleton<ImportSample>();
            services.AddSingleton<DiscoverySample>();
            services.AddSingleton<FaceScanner>();
            services.AddSingleton<VideoConverter>();
            services.AddSingleton<BulkMediaUpdater>();
            services.AddSingleton<ImageHasher>();
            services.AddSingleton<ClientThumbprintLoader>();

            //services.AddSingleton<IGeoDecoderService>(p =>
            //{
            //    return new GeoDecoderCacheStore(p.GetRequiredService<MediaStoreContext>(),
            //        new BingMapsGeoDecoderService(p.GetRequiredService<BingMapsOptions>()));
            //});

            services.AddSingleton<IGeoDecoderService>(p =>
            {
                return new GeoDecoderCacheStore(p.GetRequiredService<MediaStoreContext>(),
                    new GoogleMapsGeoDecoderService());
            });

            services.AddSingleton<IUserContextFactory, NoOpUserContextFactory>();
            services.AddMemoryCache();
            return services.BuildServiceProvider();
        }
    }
}
