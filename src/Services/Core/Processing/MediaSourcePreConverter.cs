using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using MagicMedia.Configuration;
using MagicMedia.Discovery;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace MagicMedia.Processing;

public class MediaSourcePreConverter : IMediaSourcePreConverter
{
    private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
    private readonly FileSystemStoreOptions _storeOptions;
    private readonly ILogger<MediaSourcePreConverter> _logger;
    private readonly FileSystemDiscoveryOptions _options;

    public MediaSourcePreConverter(
        IMediaSourceDiscoveryFactory discoveryFactory,
        FileSystemStoreOptions storeOptions,
        ILogger<MediaSourcePreConverter> logger,
        FileSystemDiscoveryOptions options)
    {
        _discoveryFactory = discoveryFactory;
        _storeOptions = storeOptions;
        _logger = logger;
        _options = options;
    }

    public async Task ProConvertAsync(
        CancellationToken cancellationToken)
    {
        App.ActivitySource.StartActivity("PreConvertMedia");

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

            _logger.MediaFoundforPreConversion(todo.Count);
        }

        foreach (MediaDiscoveryIdentifier file in todo.ToList())
        {
            await ConvertHiecToJpgAsync(file, cancellationToken);
        }
    }

    private async Task ConvertHiecToJpgAsync(MediaDiscoveryIdentifier file, CancellationToken cancellationToken)
    {
        Activity? activity = App.ActivitySource.StartActivity($"Convert from HIEC zo JPG: {file.Id}");
        try
        {
            using var image = new MagickImage(file.Id);

            var jpegPath = Path.ChangeExtension(file.Id, ".jpg");
            await image.WriteAsync(jpegPath, MagickFormat.Jpeg, cancellationToken);

            activity?.AddEvent(new ActivityEvent($"Written to: {jpegPath}"));

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
            activity?.AddEvent(new ActivityEvent($"Original moved to: {newFilePath}"));

        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
        }

    }
}
