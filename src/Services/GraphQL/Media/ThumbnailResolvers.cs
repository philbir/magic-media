using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Extensions;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    internal class ThumbnailResolvers
    {
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public ThumbnailResolvers(IThumbnailBlobStore thumbnailBlobStore)
        {
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

        public async Task<MediaThumbnail> GetThumbnailAsync(
            Media media,
            ThumbnailByMediaIdDataLoader thumbnailLoader,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            return await thumbnailLoader.LoadAsync(
                new Tuple<Guid, ThumbnailSizeName>(media.Id, size),
                cancellationToken);
        }
    }
}
