using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using Microsoft.Extensions.Configuration;
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


        public Stream GetStreamAsync(
            MediaBlobData request)
        {
            var filename = GetFilename(request);

            return new FileStream(filename, FileMode.Open);
        }

        public async Task StoreAsync(MediaBlobData data, CancellationToken cancellationToken)
        {
            var filename = GetFilename(data);
            var file = new FileInfo(filename);

            if (!file.Directory!.Exists)
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            await File.WriteAllBytesAsync(filename, data.Data, cancellationToken);
        }

        public Task MoveAsync(
            MediaBlobData request,
            string newLocation,
            CancellationToken cancellationToken)
        {
            var filename =  GetFilename(request);

            var newPathFragments = new List<string> { _options.RootDirectory };
            newPathFragments.AddRange(newLocation.Split('/'));

            var newDir = Path.Combine(newPathFragments.ToArray());

            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            var newFilename = Path.Combine(newDir, Path.GetFileName(filename));

            File.Move(filename, newFilename, true);

            return Task.CompletedTask;
        }

        public Task MoveToSpecialFolderAsync(
            MediaBlobData request,
            MediaBlobType mediaBlobType,
            CancellationToken cancellationToken)
        {
            var filname = GetFilename(request);

            var newDir = GetDirectory(new MediaBlobData
            {
                Type = mediaBlobType
            });

           
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            var newFilename = Path.Combine(newDir, Path.GetFileName(filname));

            File.Move(filname, newFilename, true);

            return Task.CompletedTask;
        }

        public string GetFilename(MediaBlobData data)
        {
            string? directory = GetDirectory(data);

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

            if (data.Directory != null)
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
