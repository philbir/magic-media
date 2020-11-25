using System.Collections.Generic;
using MagicMedia.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Stores
{
    public static class FileSystemStoreServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddFileSystemStore(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddFileSystemStore(builder.Configuration);

            return builder;
        }

        public static IServiceCollection AddFileSystemStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            FileSystemStoreOptions options = configuration.GetSection("MagicMedia:FileSystemStore")
                .Get<FileSystemStoreOptions>();

            if (options.BlobTypeMap == null || options.BlobTypeMap.Count == 0)
            {
                options.BlobTypeMap = GetDefaultMap();
            }

            services.AddSingleton(options);
            services.AddSingleton<IMediaBlobStore, FileSystemMediaBlobStore>();

            return services;
        }

        private static Dictionary<MediaBlobType, string> GetDefaultMap()
        {
            return new Dictionary<MediaBlobType, string>
            {
                [MediaBlobType.Media] = "/",
                [MediaBlobType.Recycled] = "System/Deleted",
                [MediaBlobType.Duplicate] = "System/Duplicate",
                [MediaBlobType.Imported] = "System/Imported",
                [MediaBlobType.Inbox] = "System/Inbox",
                [MediaBlobType.VideoPreview] = "System/VideoPreview",
                [MediaBlobType.Web] = "System/Web",
            };
        }
    }
}
