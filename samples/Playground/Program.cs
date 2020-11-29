using System;
using System.IO;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Messaging;
using MagicMedia.Playground;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            IServiceProvider sp = BuildServiceProvider();
            BulkMediaUpdater updater = sp.GetService<BulkMediaUpdater>();
            VideoConverter videoConverter = sp.GetService<VideoConverter>();

            //await videoConverter.GenerateVideosAsync(default);

            await updater.CleanUpDeletedAsync(default);
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
                .AddWorkerMessaging();

            services.AddSingleton<ImportSample>();
            services.AddSingleton<DiscoverySample>();
            services.AddSingleton<FaceScanner>();
            services.AddSingleton<VideoConverter>();
            services.AddSingleton<BulkMediaUpdater>();

            return services.BuildServiceProvider();
        }
    }
}
