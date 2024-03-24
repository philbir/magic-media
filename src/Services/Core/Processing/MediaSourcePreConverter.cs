using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using MagicMedia.Configuration;
using MagicMedia.Discovery;

namespace MagicMedia.Processing;

public class MediaSourcePreConverter : IMediaSourcePreConverter
{
    private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
    private readonly FileSystemStoreOptions _storeOptions;
    private readonly FileSystemDiscoveryOptions _options;

    public MediaSourcePreConverter(
        IMediaSourceDiscoveryFactory discoveryFactory,
        FileSystemStoreOptions storeOptions,
        FileSystemDiscoveryOptions options)
    {
        _discoveryFactory = discoveryFactory;
        _storeOptions = storeOptions;
        _options = options;
    }

    public async Task ProConvertAsync(
        CancellationToken cancellationToken)
    {
        var todo = new List<MediaDiscoveryIdentifier>();

        foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
        {
            IEnumerable<FileDiscoveryLocation> heicLocs = _options.Locations
                .Where(x => x.Filter.ToLower() == "*.jpg")
                .Select(l => new FileDiscoveryLocation { Root = l.Root, Path = l.Path, Filter = "*.heic" })
                .ToList();

            IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                .DiscoverMediaAsync(
                    new FileSystemDiscoveryOptions { Locations = heicLocs },
                    cancellationToken);

            todo.AddRange(identifiers);
        }

        foreach (MediaDiscoveryIdentifier file in todo.ToList())
        {
            await ConvertHiecToJpgAsync(file, cancellationToken);
        }
    }

    private async Task ConvertHiecToJpgAsync(MediaDiscoveryIdentifier file, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(file.Id);

        var jpegPath = Path.ChangeExtension(file.Id, ".jpg");
        await image.WriteAsync(jpegPath, MagickFormat.Jpeg, cancellationToken);

        //Move File
        var newLocation = Path.Combine(_storeOptions.RootDirectory, "HIEC_Archive");
        if (!Directory.Exists(newLocation))
        {
            Directory.CreateDirectory(newLocation);
        }
        var newFilePath = Path.Combine(
            newLocation,
            $"{Guid.NewGuid().ToString().Substring(0,4)}_{Path.GetFileName(file.Id)}");

        File.Move(file.Id, newFilePath);
    }
}
