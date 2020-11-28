using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Video;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class ExtractVideoDataTask : IMediaProcessorTask
    {
        private readonly IVideoProcessingService _videoProcessingService;

        public string Name => MediaProcessorTaskNames.ExtractVideoData;

        public ExtractVideoDataTask(IVideoProcessingService videoProcessingService)
        {
            _videoProcessingService = videoProcessingService;
        }

        public async Task ExecuteAsync(MediaProcessorContext context, CancellationToken cancellationToken)
        {
            ExtractVideoDataResult? videoData = await _videoProcessingService.ExtractVideoDataAsync(
                context.File.Id,
                cancellationToken);

            context.Image = Image.Load(videoData.ImageData);
            context.Metadata = videoData.Meta;
            context.VideoInfo = videoData.Info;
            context.Size = videoData.Size;
        }
    }
}
