using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Discovery
{
    public static class MediaDiscoveryServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystemDiscovery(
            this IServiceCollection services,
            IEnumerable<string> locations)
        {
            services.AddSingleton<IMediaSourceDiscovery, FileSystemSourceDiscovery>();
            services.AddSingleton<IMediaSourceDiscoveryFactory, MediaSourceDiscoveryFactory>();

            services.AddSingleton(new FileSystemDiscoveryOptions
            {
                Locations = locations
            });

            return services;
        }
    }
}
