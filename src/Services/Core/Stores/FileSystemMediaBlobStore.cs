using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Stores;

public class FileSystemMediaBlobStore(
    FileSystemStoreOptions options,
    ILogger<FileSystemMediaBlobStore> logger) : IMediaBlobStore
{
    public async Task<MediaBlobData> GetAsync(
        MediaBlobData request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = Tracing.Source.StartActivity("Get MediaBlobData");

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

    public Task<string> MoveAsync(
        MediaBlobData request,
        string newLocation,
        CancellationToken cancellationToken)
    {
        var existingFilename = GetFilename(request);

        var newPathFragments = new List<string> { options.RootDirectory };
        newPathFragments.AddRange(newLocation.Split('/'));

        var newDir = Path.Combine(newPathFragments.ToArray());

        if (!Directory.Exists(newDir))
        {
            Directory.CreateDirectory(newDir);
        }

        var name = Path.GetFileName(existingFilename);

        var newPath = Path.Combine(newDir, name );

        //Exists handling
        if (File.Exists(newPath))
        {
            name = GetNewFilename(newDir, Path.GetFileName(existingFilename));
            newPath = Path.Combine(newDir, name);
        }

        File.Move(existingFilename, newPath, false);

        return Task.FromResult(name);
    }

    private string GetNewFilename(string path, string filename)
    {
        var nr = 1;

        while (nr < 10)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var newName = $"{name}_{nr}{Path.GetExtension(filename)}";

            if (!File.Exists(Path.Combine(path,newName)))
            {
                return newName;
            }

            nr++;
        }

        throw new ApplicationException("No no filename could be found");
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

    public Task<bool> DeleteAsync(
        MediaBlobData request,
        CancellationToken cancellationToken)
    {
        var filename = GetFilename(request);
        if (File.Exists(filename))
        {
            File.Delete(filename);
            logger.LogInformation("Delete {File}", filename);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public string GetFilename(MediaBlobData data)
    {
        string? directory = GetDirectory(data);

        return Path.Combine(directory, data.Filename);
    }

    private string GetDirectory(MediaBlobData data)
    {
        var loc = options.BlobTypeMap[data.Type];
        var paths = new List<string>
            {
                options.RootDirectory
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
