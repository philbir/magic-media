using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
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

        public string GetDataUrl(
            [Parent] Media media,
            MediaThumbnail thumbnail)
        {
            if (thumbnail.Data != null)
            {
                return thumbnail.Data.ToDataUrl(thumbnail.Format);
            }
            else
            {
                return $"api/media/{media.Id}/thumbnailbyid/{thumbnail.Id}";
            }

            //if (thumbnail.Data == null)
            //{
            //    thumbnail.Data = await _thumbnailBlobStore.GetAsync(
            //        thumbnail.Id,
            //        cancellationToken);
            //}
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(
            Media media,
            ThumbnailDataDataLoader thumbnailLoader,
            ThumbnailSizeName size,
            bool loadData,
            CancellationToken cancellationToken)
        {
            MediaThumbnail? thumb = _mediaService.GetThumbnail(media, size);

            if (thumb != null)
            {
                if (loadData)
                {
                    return await thumbnailLoader.LoadAsync(thumb, cancellationToken);
                }

                return thumb;
            }

            return null;
        }
    }
}
