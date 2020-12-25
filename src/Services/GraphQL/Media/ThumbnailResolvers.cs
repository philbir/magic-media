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

        public ThumbnailResolvers(
            IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public string GetDataUrl(
            MediaThumbnail thumbnail)
        {
            if (thumbnail.Data != null)
            {
                return thumbnail.Data.ToDataUrl(thumbnail.Format);
            }
            else
            {
                string url = "";

                switch (thumbnail.Owner.Type)
                {
                    case ThumbnailOwnerType.Media:
                        url = $"/api/media/{thumbnail.Owner.Id}/thumbnailbyid/{thumbnail.Id}";
                        break;
                    case ThumbnailOwnerType.Face:
                        url = $"/api/face/{thumbnail.Owner.Id}/thumbnail/{thumbnail.Id}";
                        break;
                }

                return url;
            }
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
                thumb.Owner = new ThumbnailOwner
                {
                    Type = ThumbnailOwnerType.Media,
                    Id = media.Id
                };

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
