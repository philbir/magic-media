using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Discovery;

public class FileSystemSourceDiscovery(ILogger<FileSystemSourceDiscovery> logger) : IMediaSourceDiscovery
{
    public MediaDiscoverySource SourceType => MediaDiscoverySource.FileSystem;

    public Task<IEnumerable<MediaDiscoveryIdentifier>> DiscoverMediaAsync(
        FileSystemDiscoveryOptions options,
        CancellationToken cancellationToken)
    {
        var result = new List<MediaDiscoveryIdentifier>();

        foreach (FileDiscoveryLocation location in options.Locations)
        {
            var filePath = location.Root != null ? Path.Combine(location.Root, location.Path) : location.Path;

            var pattern = location.Filter ?? "*.*";
            logger.DiscoverMediaWithPathAndPattern(filePath, pattern);

            string[] files;

            if (location.Filter is { } f && f != "*.*" && f.StartsWith("*."))
            {
                files = GetFilesByExtension(filePath, pattern).ToArray();
            }
            else
            {
                files = Directory.GetFiles(
                    filePath,
                    pattern,
                    SearchOption.AllDirectories)
                    .ToArray();
            }

            logger.MediaFoundWithPathAndPattern(files.Length, filePath, pattern);

            result.AddRange(files.Select(x =>
                new MediaDiscoveryIdentifier { BasePath = location.Root ?? filePath, Id = x, Source = SourceType }));
        }

        return Task.FromResult(result.Distinct());
    }

    private IEnumerable<string> GetFilesByExtension(string path, string extension)
    {
        foreach (var file in Directory.EnumerateFiles(path, extension.ToLower(), SearchOption.AllDirectories))
        {
            yield return file;
        }

        foreach (var file in Directory.EnumerateFiles(path, extension.ToUpper(), SearchOption.AllDirectories))
        {
            yield return file;
        }
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
        var file = new FileInfo(id);

        if (file.Directory is { } && file.Directory.Exists)
        {
            file.Directory.Create();
        }

        await File.WriteAllBytesAsync(id, data, cancellationToken);
    }
}
