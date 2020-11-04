using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public async Task ScanExistingAsync(CancellationToken cancellationToken)
        {
            var todo = new List<MediaDiscoveryIdentifier>();

            foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
            {
                IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                    .DiscoverMediaAsync(default);

                todo.AddRange(identifiers);
            }

            IMediaProcessorFlow flow = _flowFactory.CreateFlow("ImportImageNoFace");

            var options = new MediaProcessingOptions
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

                byte[] data = await src.GetMediaDataAsync(file.Id, default);

                var context = new MediaProcessorContext
                {
                    OriginalData = data,
                    File = file,
                    Options = options
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
            }
        }


        public async Task DiscoverAsync()
        {
            var todo = new List<MediaDiscoveryIdentifier>();

            foreach (IMediaSourceDiscovery source in _discoveryFactory.GetSources())
            {
                IEnumerable<MediaDiscoveryIdentifier> identifiers = await source
                    .DiscoverMediaAsync(default);

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
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                //TODO: Move to Imported or delete
            }
        }
    }
}
