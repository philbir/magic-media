using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Store;

namespace MagicMedia.Processing
{
    public class MediaSourceScanner : IMediaSourceScanner
    {
        private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
        private readonly IMediaProcessorFlowFactory _flowFactory;
        private readonly FileSystemDiscoveryOptions _options;
        private readonly IMediaProcessorFlow _imageFlow;
        private readonly IMediaProcessorFlow _videoFlow;
        static Dictionary<string, MediaType> _fileTypeMap = new()
        {
            [".jpg"] = MediaType.Image,
            [".png"] = MediaType.Image,
            [".mp4"] = MediaType.Video,
            [".mov"] = MediaType.Video,
        };

        public MediaSourceScanner(
            IMediaSourceDiscoveryFactory discoveryFactory,
            IMediaProcessorFlowFactory flowFactory,
            FileSystemDiscoveryOptions options)
        {
            _discoveryFactory = discoveryFactory;
            _flowFactory = flowFactory;
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
        }
    }
}
