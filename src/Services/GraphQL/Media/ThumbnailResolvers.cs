using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    internal class ThumbnailResolvers
    {
        private readonly IMediaService _mediaService;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public ThumbnailResolvers(
            IMediaService mediaService,
            IThumbnailBlobStore thumbnailBlobStore)
        {
            _mediaService = mediaService;
            _thumbnailBlobStore = thumbnailBlobStore;
        }

        public async Task<string> GetDataUrl(
            MediaThumbnail thumbnail,
            CancellationToken cancellationToken)
        {
            if (thumbnail.Data == null)
            {
                thumbnail.Data = await _thumbnailBlobStore.GetAsync(
                    thumbnail.Id,
                    cancellationToken);
            }

            return thumbnail.Data.ToDataUrl(thumbnail.Format);
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(
            Media media,
            ThumbnailDataDataLoader thumbnailLoader,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            MediaThumbnail? thumb = _mediaService.GetThumbnail(media, size);

            if ( thumb != null)
            {
                return await thumbnailLoader.LoadAsync(thumb, cancellationToken);
            }

            return null;
        }
    }
}
