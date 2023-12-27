using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public class VideoPlayerService(
    IMediaStore mediaStore,
    IMediaService mediaService) : IVideoPlayerService
{
    public MediaStream GetVideoPreview(Guid id, CancellationToken cancellationToken)
    {
        MediaBlobData request = mediaService.GetBlobRequest(
            new Media { Id = id },
            MediaFileType.VideoGif);

        Stream stream = mediaStore.Blob.GetStreamAsync(request);

        return new MediaStream(stream, "image/gif");
    }

    public async Task<MediaStream> GetVideoAsync(Guid id, CancellationToken cancellationToken)
    {
        Media? video = await mediaStore.GetByIdAsync(id, cancellationToken);

        MediaBlobData request = mediaService.GetBlobRequest(
            video,
            MediaFileType.Video720);

        Stream stream = mediaStore.Blob.GetStreamAsync(request);

        return new MediaStream(stream, video.Filename.Split('.').Last());
    }
}
