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

                await flow.ExecuteAsync(context, default);

                //TODO: Move to Imported or delete
            }
        }
    }
}
