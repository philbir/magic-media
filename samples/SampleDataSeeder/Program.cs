using System;
using System.IO;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.BingMaps;
using MagicMedia.SampleDataSeader;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleDataSeeder
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider services = BuildServiceProvider();

            Seeder seeder = services.GetRequiredService<Seeder>();
            ApiSeeder apiSeeder = services.GetRequiredService<ApiSeeder>();

            //await seeder.RunAsync(100, default);

            await apiSeeder.SeedFromDirectoryAsync(@"C:\Users\tree\Pictures\RF", default);
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

            services.AddDataSeaders();

            return services.BuildServiceProvider();
        }
    }
}
