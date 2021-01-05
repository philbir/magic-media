using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public class MediaDownloadService : IMediaDownloadService
    {
        private readonly IMediaService _mediaService;

        public MediaDownloadService(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<MediaDownload> CreateDownloadAsync(
            Guid id,
            DownloadMediaOptions options,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

            Stream stream = _mediaService.GetMediaStream(media);

            return new MediaDownload(stream, CreateFilename(media));
        }

        private string CreateFilename(Media media)
        {
            StringBuilder sb = new StringBuilder();
            if (media.Folder != null)
            {
                var folders = media.Folder.Split('/', StringSplitOptions.RemoveEmptyEntries);

                sb.Append(string.Join("_", folders));
                sb.Append("_");
            }

            sb.Append(media.Filename);

            return RemoveIllegalChars(sb.ToString());
        }

        private string RemoveIllegalChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
