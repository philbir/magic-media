using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Stores
{
    public class FileSystemMediaBlobStore : IMediaBlobStore

    {
        private readonly FileSystemStoreOptions _options;

        public FileSystemMediaBlobStore(FileSystemStoreOptions options)
        {
            _options = options;
        }

        public async Task<MediaBlobData> GetAsync(
            MediaBlobData request,
            CancellationToken cancellationToken)
        {
            var filename = GetFilename(request);
            byte[] data = await File.ReadAllBytesAsync(filename, cancellationToken);

            return request with { Data = data };
        }

        public async Task StoreAsync(MediaBlobData data, CancellationToken cancellationToken)
        {
            var filename = GetFilename(data);

            await File.WriteAllBytesAsync(filename, data.Data, cancellationToken);
        }

        private string GetFilename(MediaBlobData data)
        {
            var directory = GetDirectory(data);
            return Path.Combine(directory, data.Filename);
        }

        private string GetDirectory(MediaBlobData data)
        {
            var loc = _options.BlobTypeMap[data.Type];
            var paths = new List<string>
            {
                _options.RootDirectory
            };
            paths.AddRange(loc.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

            if ( data.Directory != null)
            {
                paths.AddRange(data.Directory.Split(
                    new[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries));
            }

            return Path.Combine(paths.ToArray());
        }
    }


    public static class FileSystemStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystemStore(
            this IServiceCollection services,
            string rootDirectory)
        {
            var options = new FileSystemStoreOptions
            {
                RootDirectory = rootDirectory,
                BlobTypeMap = GetDefaultMap()
            };

            services.AddSingleton(options);
            services.AddSingleton<IMediaBlobStore, FileSystemMediaBlobStore>();

            return services;
        }

        private static Dictionary<MediaBlobType, string> GetDefaultMap()
        {
            return new Dictionary<MediaBlobType, string>
            {
                [MediaBlobType.Deleted] = "System/Deleted",
                [MediaBlobType.Duplicate] = "System/Duplicate",
                [MediaBlobType.Imported] = "System/Imported",
                [MediaBlobType.Inbox] = "System/Inbox",
                [MediaBlobType.VideoPreview] = "System/VideoPreview",
                [MediaBlobType.Web] = "System/Web",
            };
        }
    }
}
