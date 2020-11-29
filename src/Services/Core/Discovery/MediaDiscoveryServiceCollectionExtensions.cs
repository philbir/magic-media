using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Discovery
{
    public static class MediaDiscoveryServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddFileSystemDiscovery(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddSingleton<IMediaSourceDiscovery, FileSystemSourceDiscovery>();
            builder.Services.AddSingleton<IMediaSourceDiscoveryFactory, MediaSourceDiscoveryFactory>();

            return builder;
        }
    }
}
