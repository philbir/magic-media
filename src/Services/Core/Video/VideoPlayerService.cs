using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public class VideoPlayerService : IVideoPlayerService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaService _mediaService;

        public VideoPlayerService(
            IMediaStore mediaStore,
            IMediaService mediaService)
        {
            _mediaStore = mediaStore;
            _mediaService = mediaService;
        }

        public MediaStream GetVideoPreview(Guid id, CancellationToken cancellationToken)
        {
            MediaBlobData request = _mediaService.GetBlobRequest(
                new Media { Id = id },
                MediaFileType.VideoGif);

            Stream stream = _mediaStore.Blob.GetStreamAsync(request);

            return new MediaStream(stream, "image/gif");
        }

        public async Task<MediaStream> GetVideoAsync(Guid id, CancellationToken cancellationToken)
        {
            Media? video = await _mediaStore.GetByIdAsync(id, cancellationToken);

            MediaBlobData request = _mediaService.GetBlobRequest(
                video,
                MediaFileType.Video720);

            Stream stream = _mediaStore.Blob.GetStreamAsync(request);

            return new MediaStream(stream, video.Filename.Split('.').Last());
        }
    }
}
