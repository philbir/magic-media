using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using MagicMedia.Configuration;
using MagicMedia.Discovery;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using Activity = System.Diagnostics.Activity;

namespace MagicMedia.Processing;

public partial class Meters
{
    static Meter _meter = new Meter("magicmedia.core.processing");
    private static Histogram<double> _hiecConversionTime = _meter.CreateHistogram<double>(
        _meter.Name + ".hiec_conversion_time",
        "The time it takes to convert a hiec file to jpg",
        "ms");

    internal static void RecordHiecConversionTime(double time)
    {
        _hiecConversionTime.Record(time);
    }
}


public class MediaSourcePreConverter : IMediaSourcePreConverter
{
    private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
    private readonly FileSystemStoreOptions _storeOptions;
    private readonly FileSystemDiscoveryOptions _options;
    private readonly ILogger<MediaSourcePreConverter> _logger;
    private static ActivitySource _source = new ActivitySource("MagicMedia.Core.MediaSourcePreConverter");

    public MediaSourcePreConverter(
        IMediaSourceDiscoveryFactory discoveryFactory,
        FileSystemStoreOptions storeOptions,
        FileSystemDiscoveryOptions options,
        ILogger<MediaSourcePreConverter> logger)
    {
        _discoveryFactory = discoveryFactory;
        _storeOptions = storeOptions;
        _options = options;
        _logger = logger;
    }

    public async Task PreConvertAsync(
        CancellationToken cancellationToken)
    {
        using Activity? activity = _source.StartActivity("PreConvertFiles");
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

            activity?.SetTag("filesToConvert", todo.Count());
        }

        foreach (MediaDiscoveryIdentifier file in todo.ToList())
        {
            try
            {
                var sw = Stopwatch.StartNew();
                await ConvertHiecToJpgAsync(file, cancellationToken);

                Meters.RecordHiecConversionTime(sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.ErrorConvertingHiecFile(file.Id, ex);
            }
        }
    }

    private async Task ConvertHiecToJpgAsync(MediaDiscoveryIdentifier file, CancellationToken cancellationToken)
    {
        using Activity? activity = _source.StartActivity("ConvertHiecToJpg");
        activity?.SetTag("file", file.Id);
        _logger.ConvertingHiecFile(file.Id);

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

public static partial class MediaSourcePreConverterLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Converting HIEC file: {file}")]
    public static partial void ConvertingHiecFile(this ILogger logger, string file);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error converting HIEC file: {file} - {ex}")]
    public static partial void ErrorConvertingHiecFile(this ILogger logger, string file, Exception ex);
}
