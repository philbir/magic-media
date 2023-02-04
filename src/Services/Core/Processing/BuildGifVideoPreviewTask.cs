using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Video;

namespace MagicMedia.Processing;

public class BuildGifVideoPreviewTask : IMediaProcessorTask
{
    private readonly IMediaService _mediaService;
    private readonly IVideoProcessingService _videoProcessingService;

    public string Name => MediaProcessorTaskNames.BuildGifVideoPreview;

    public BuildGifVideoPreviewTask(
        IMediaService mediaService,
        IVideoProcessingService videoProcessingService)
    {
        _mediaService = mediaService;
        _videoProcessingService = videoProcessingService;
    }

    public async Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken)
    {
        var filename = _mediaService.GetFilename(context.Media, MediaFileType.Original);
        var gifFilename = _mediaService.GetFilename(context.Media, MediaFileType.VideoGif);

        if (File.Exists(gifFilename))
        {
            File.Delete(gifFilename);
        }

        await _videoProcessingService.GeneratePreviewGifAsync(
            filename,
            gifFilename,
            cancellationToken);
    }
}
