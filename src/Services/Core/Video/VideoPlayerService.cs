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

        public VideoPlayerService(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public MediaStream GetVideoPreview(Guid id, CancellationToken cancellationToken)
        {
            Stream stream = _mediaStore.Blob.GetStreamAsync(new MediaBlobData
            {
                Type = MediaBlobType.VideoPreview,
                Filename = $"{id}.gif"
            });

            return new MediaStream(stream, "image/gif");
        }

        public async Task<MediaStream> GetVideoAsync(Guid id, CancellationToken cancellationToken)
        {
            Media? video = await _mediaStore.GetByIdAsync(id, cancellationToken);

            //Stream stream = _mediaStore.Blob.GetStreamAsync(new MediaBlobData
            //{
            //    Type = MediaBlobType.Media,
            //    Filename = video.Filename,
            //    Directory = video.Folder!
            //});

            Stream stream = _mediaStore.Blob.GetStreamAsync(new MediaBlobData
            {
                Type = MediaBlobType.VideoPreview,
                Filename = $"720P_{id}.mp4"
            });

            return new MediaStream(stream, video.Filename.Split('.').Last());
        }
    }
}
