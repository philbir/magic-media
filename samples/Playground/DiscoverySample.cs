using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Processing;
using MagicMedia.Store;

namespace MagicMedia.Playground
{
    public class DiscoverySample
    {
        private readonly IMediaSourceDiscoveryFactory _discoveryFactory;
        private readonly IMediaProcessorFlowFactory _flowFactory;
        private readonly IMediaStore _store;

        public DiscoverySample(
            IMediaSourceDiscoveryFactory discoveryFactory,
            IMediaProcessorFlowFactory flowFactory,
            IMediaStore store)
        {
            _discoveryFactory = discoveryFactory;
            _flowFactory = flowFactory;
            _store = store;
        }

        public async Task ScanExistingAsync(
            FileSystemDiscoveryOptions options,
            CancellationToken cancellationToken)
        {
            var todo = new List<MediaDiscoveryIdentifier>();

            foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
            {
                IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                    .DiscoverMediaAsync(options, cancellationToken);

                todo.AddRange(identifiers);
            }

            IMediaProcessorFlow imageFlow = _flowFactory.CreateFlow("ImportImageNoFace");
            IMediaProcessorFlow videoFlow = _flowFactory.CreateFlow("ImportVideo");

            var processionOptions = new MediaProcessingOptions
            {
                SaveMedia = new SaveMediaFileOptions
                {
                    SaveMode = SaveMediaMode.KeepInSource,
                    SourceAction = SaveMediaSourceAction.Keep
                }
            };


            foreach (MediaDiscoveryIdentifier file in todo)
            {
                IMediaSourceDiscovery src = _discoveryFactory.GetSource(file.Source);

                var extension = Path.GetExtension(file.Id);

                if (extension == ".mp4")
                {
                    var context = new MediaProcessorContext
                    {
                        File = file,
                        Options = processionOptions,
                        MediaType = MediaType.Video
                    };
                    try
                    {
                        Console.WriteLine($"Importing vide: {file.Id}");
                        await videoFlow.ExecuteAsync(context, default);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    byte[] data = await src.GetMediaDataAsync(file.Id, default);

                    var context = new MediaProcessorContext
                    {
                        OriginalData = data,
                        File = file,
                        Options = processionOptions,
                        MediaType = MediaType.Image
                    };
                    try
                    {
                        Console.WriteLine($"Importing: {file.Id}");
                        await imageFlow.ExecuteAsync(context, default);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public async Task DiscoverAsync(
            FileSystemDiscoveryOptions options,
            CancellationToken cancellationToken)
        {
            var todo = new List<MediaDiscoveryIdentifier>();

            foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
            {
                IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                    .DiscoverMediaAsync(options, cancellationToken);

                todo.AddRange(identifiers);
            }

            IMediaProcessorFlow flow = _flowFactory.CreateFlow("ImportImage");

            foreach (MediaDiscoveryIdentifier file in todo)
            {
                IMediaSourceDiscovery src = _discoveryFactory.GetSource(file.Source);

                byte[] data = await src.GetMediaDataAsync(file.Id, default);

                var context = new MediaProcessorContext
                {
                    OriginalData = data,
                    File = file
                };
                try
                {
                    Console.WriteLine($"Importing: {file.Id}");
                    await flow.ExecuteAsync(context, default);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                //TODO: Move to Imported or delete
            }
        }
    }
}
