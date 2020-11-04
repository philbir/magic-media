using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Face;
using MagicMedia.Playground;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider sp = BuildServiceProvider();
            ImportSample importSample = sp.GetService<ImportSample>();
            IFaceModelBuilderService modelBuilder = sp.GetService<IFaceModelBuilderService>();

            DiscoverySample discovery = sp.GetService<DiscoverySample>();

            await discovery.ScanExistingAsync(default);

        }


        private static IServiceProvider BuildServiceProvider()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddUserSecrets<Program>(optional: true)
                 .Build();

            BingMapsOptions bingOptions = config.GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            var services = new ServiceCollection();
            services.AddMongoDbStore(config);

            services.AddFileSystemStore(config);
            services.AddMagicMedia(config);
            services.AddBingMaps(bingOptions);

            services.Decorate<IGeoDecoderService, GeoDecoderCacheStore>();

            services.AddSingleton<ImportSample>();
            services.AddSingleton<DiscoverySample>();
            services.AddFileSystemDiscovery(new List<string> { @"P:\Moments\Family\2020" });

            return services.BuildServiceProvider();
        }
    }
}
