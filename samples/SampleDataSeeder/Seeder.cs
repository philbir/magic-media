using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Processing;

namespace SampleDataSeeder
{
    public class Seeder
    {
        private readonly IEnumerable<ISampleDataSource> _sources;
        private readonly IMediaProcessorFlowFactory _processorFlowFactory;

        public Seeder(
            IEnumerable<ISampleDataSource> sources,
            IMediaProcessorFlowFactory processorFlowFactory)
        {
            _sources = sources;
            _processorFlowFactory = processorFlowFactory;
        }


        public async Task RunAsync(int count, CancellationToken cancellationToken)
        {
            IMediaProcessorFlow flow = _processorFlowFactory.CreateFlow("ImportImage");

            int todo = count;

            while (todo > 0)
            {
                foreach (ISampleDataSource source in _sources)
                {
                    SampleMedia media = await source.LoadAsync(cancellationToken);
                    await flow.ExecuteAsync(new MediaProcessorContext
                    {
                        File = new MediaDiscoveryIdentifier
                        {
                            Id = media.Filename,
                            Source = MediaDiscoverySource.WebUpload
                        },
                        OriginalData = media.Data
                    }, cancellationToken);
                }
            }
        }
    }
}
