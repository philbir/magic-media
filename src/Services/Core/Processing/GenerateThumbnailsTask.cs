using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Thumbnail;

namespace MagicMedia.Processing
{
    public class GenerateThumbnailsTask : IMediaProcesserTask
    {
        private readonly IThumbnailService _thumbnailService;

        public string Name => MediaProcessorTaskNames.GenerateThumbnails;

        public GenerateThumbnailsTask(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            context.Thumbnails = await _thumbnailService.GenerateAllThumbnailAsync(
                context.Image,
                cancellationToken);
        }
    }
}
