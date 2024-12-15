using System;
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
            var filePath = location.Root != null ?
                Path.Combine(location.Root, location.Path) :
                location.Path;

            string pattern = location.Filter ?? "*.*";
            logger.DiscoverMediaInPath(filePath, pattern);

            string[] files = Directory.GetFiles(
                filePath,
                pattern,
                new EnumerationOptions
                {
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = true
                });

            logger.MediaFoundInPath(files.Length, filePath);

            result.AddRange(files.Select(x =>
                new MediaDiscoveryIdentifier
                {
                    BasePath = location.Root ?? filePath,
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

public static partial class FileSystemSourceDiscoveryLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Discover media in {Path} with pattern: {Pattern}")]
    public static partial void DiscoverMediaInPath(this ILogger logger, string path, string pattern);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "{Count} media found in {Path}")]
    public static partial void MediaFoundInPath(this ILogger logger, int count, string path);
}
