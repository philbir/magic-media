using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Store;
using Serilog;

namespace MagicMedia.Processing;

public class MediaSourceScanner : IMediaSourceScanner
{
    private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
    private readonly IMediaProcessorFlowFactory _flowFactory;
    private readonly IDuplicateMediaGuard _duplicateMediaGuard;
    private readonly FileSystemDiscoveryOptions _options;
    private readonly IMediaProcessorFlow _imageFlow;
    private readonly IMediaProcessorFlow _videoFlow;
    static Dictionary<string, MediaType> _fileTypeMap = new()
    {
        [".jpg"] = MediaType.Image,
        [".jpeg"] = MediaType.Image,
        [".heic"] = MediaType.Image,
        [".png"] = MediaType.Image,
        [".mp4"] = MediaType.Video,
        [".mov"] = MediaType.Video,
    };

    public MediaSourceScanner(
        IMediaSourceDiscoveryFactory discoveryFactory,
        IMediaProcessorFlowFactory flowFactory,
        IDuplicateMediaGuard duplicateMediaGuard,
        FileSystemDiscoveryOptions options)
    {
        _discoveryFactory = discoveryFactory;
        _flowFactory = flowFactory;
        _duplicateMediaGuard = duplicateMediaGuard;
        _options = options;
        _imageFlow = _flowFactory.CreateFlow("ImportImage");
        _videoFlow = _flowFactory.CreateFlow("ImportVideo");
    }

    private async Task<IEnumerable<MediaDiscoveryIdentifier>> DiscoverMediaAsync(
        CancellationToken cancellationToken)
    {
        var todo = new List<MediaDiscoveryIdentifier>();

        foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
        {
            IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                .DiscoverMediaAsync(_options, cancellationToken);

            todo.AddRange(identifiers);
        }

        return todo;
    }

    public async Task ScanAsync(CancellationToken cancellationToken)
    {
        await _duplicateMediaGuard.InitializeAsync(cancellationToken);

        IEnumerable<MediaDiscoveryIdentifier> files = await DiscoverMediaAsync(
            cancellationToken);

        foreach (MediaDiscoveryIdentifier? file in files)
        {
            await ProcessFileAsync(file, cancellationToken);
        }
    }

    public async Task ProcessFileAsync(
        MediaDiscoveryIdentifier file,
        CancellationToken cancellationToken)
    {
        IMediaSourceDiscovery src = _discoveryFactory.GetSource(file.Source);
        var extension = Path.GetExtension(file.Id).ToLower();

        if (_fileTypeMap.ContainsKey(extension))
        {
            MediaType mediaType = _fileTypeMap[extension];

            var context = new MediaProcessorContext
            {
                Guard = _duplicateMediaGuard,
                File = file,
                Options = new MediaProcessingOptions
                {
                    SaveMedia = new SaveMediaFileOptions
                    {
                        SaveMode = SaveMediaMode.CreateNew,
                        SourceAction = SaveMediaSourceAction.Delete
                    }
                },
                MediaType = mediaType
            };

            try
            {
                switch (mediaType)
                {
                    case MediaType.Image:
                        context.OriginalData = await src.GetMediaDataAsync(
                            file.Id,
                            cancellationToken);
                        await _imageFlow.ExecuteAsync(context, cancellationToken);
                        break;
                    case MediaType.Video:
                        await _videoFlow.ExecuteAsync(context, cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing file {file}", file.Id);
            }
        }
    }
}
