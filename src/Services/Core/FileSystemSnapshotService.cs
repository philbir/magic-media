using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMedia;

public class FileSystemSnapshotService : IFileSystemSnapshotService
{
    private readonly FileSystemStoreOptions _options;
    private readonly IMemoryCache _cache;
    private const string _snapshotFilename = "fs_snapshot.json";

    public FileSystemSnapshotService(FileSystemStoreOptions options, IMemoryCache cache)
    {
        _options = options;
        _cache = cache;
    }

    public async Task CreateAsync(CancellationToken cancellationToken)
    {
        var files = new List<MediaFileEntry>();
        var validExtensions = new[] { ".jpg", ".jpeg", ".mp4", ".mov" }.ToList();

        foreach (var entry in Directory.EnumerateFiles(_options.RootDirectory, "*", SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(entry);

            if ( !string.IsNullOrWhiteSpace(extension) && validExtensions.Contains(extension.ToLower()))
            {
                var path = entry.Replace(_options.RootDirectory, "");
                files.Add(new MediaFileEntry(Path.GetFileName(entry), Path.GetDirectoryName(path)));
            }
        }

        var snapshot = new FileSystemSnapshot { CreatedAt = DateTime.UtcNow, Entries = files };
        var json = JsonSerializer.Serialize(snapshot);

        await File.WriteAllTextAsync(
            Path.Combine(_options.RootDirectory, _snapshotFilename),
            json,
            cancellationToken);
    }

    public async Task<FileSystemSnapshot> LoadAsync(CancellationToken cancellationToken)
    {
        var cachKey = "_fs_ss";
        FileSystemSnapshot? cached = _cache.Get<FileSystemSnapshot>(cachKey);

        if (cached is { })
        {
            return cached;
        }

        var json = await File.ReadAllTextAsync(
            Path.Combine(_options.RootDirectory, _snapshotFilename),
            cancellationToken);

        FileSystemSnapshot snapShot = JsonSerializer.Deserialize<FileSystemSnapshot>(json);

        _cache.Set(cachKey, snapShot);

        return snapShot;
    }
}


