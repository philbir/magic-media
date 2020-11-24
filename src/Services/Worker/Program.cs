using System.Collections.Generic;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.Discovery;
using MagicMedia.Messaging;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json");
                    builder.AddUserSecrets<Program>(optional: true);
                    builder.AddJsonFile("appsettings.local.json", optional: true);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    BingMapsOptions bingOptions = hostContext.Configuration
                        .GetSection("MagicMedia:BingMaps")
                        .Get<BingMapsOptions>();

                    services.AddSingleton(new FileSystemDiscoveryOptions
                    {
                        Locations = new List<FileDiscoveryLocation>
                        {
                            new FileDiscoveryLocation
                            {
                                 Path = @"C:\Temp\School",
                            }
                        }
                    });

                    services.AddMongoDbStore(hostContext.Configuration);
                    services.AddFileSystemStore(hostContext.Configuration);
                    services.AddMagicMedia(hostContext.Configuration);
                    services.AddBingMaps(bingOptions);
                    services.Decorate<IGeoDecoderService, GeoDecoderCacheStore>();
                    services.AddFileSystemDiscovery();
                    services.AddMessaging(hostContext.Configuration);
                    services.AddSignalRCore();

                    services.AddHostedService<Worker>();
                });
    }
}
