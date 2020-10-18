using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
    public class ExtractMetadataTask : IMediaProcesserTask
    {
        private readonly IMetadataExtractor _metadataExtractor;

        public ExtractMetadataTask(IMetadataExtractor metadataExtractor)
        {
            _metadataExtractor = metadataExtractor;
        }

        public string Name => MediaProcessorTaskNames.ExtractMetadata;

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            context.Metadata = await _metadataExtractor.GetMetadataAsync(context.Image, default);
        }
    }
}
