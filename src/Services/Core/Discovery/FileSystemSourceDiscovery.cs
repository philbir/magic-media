using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Discovery
{
    public class FileSystemSourceDiscovery : IMediaSourceDiscovery
    {
        private readonly FileSystemDiscoveryOptions _options;

        public MediaDiscoverySource SourceType => MediaDiscoverySource.FileSystem;

        public FileSystemSourceDiscovery(FileSystemDiscoveryOptions options)
        {
            _options = options;
        }

        public Task<IEnumerable<MediaDiscoveryIdentifier>> DiscoverMediaAsync(
            CancellationToken cancellationToken)
        {
            var result = new List<MediaDiscoveryIdentifier>();

            foreach (var location in _options.Locations)
            {
                string[] files = Directory.GetFiles(location, "*.jpg", SearchOption.AllDirectories);
                result.AddRange(files.Select(x =>
                    new MediaDiscoveryIdentifier
                    {
                        BasePath = location,
                        Id = x,
                        Source = SourceType
                    }));
            }

            return Task.FromResult(result.Distinct());
        }

        public async Task<byte[]> GetMediaDataAsync(string id, CancellationToken cancellationToken)
        {
            return await File.ReadAllBytesAsync(id, cancellationToken);
        }

        public Task DeleteMediaAsync(string id, CancellationToken cancellationToken)
        {
            File.Delete(id);

            return Task.CompletedTask;
        }

        public async Task SaveMediaDataAsync(string id, byte[] data, CancellationToken cancellationToken)
        {
            FileInfo file = new FileInfo(id);

            if (file.Directory is { } && file.Directory.Exists)
            {
                file.Directory.Create();
            }

            await File.WriteAllBytesAsync(id, data, cancellationToken);
        }
    }
}
