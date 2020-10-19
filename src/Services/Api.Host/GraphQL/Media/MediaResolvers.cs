using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Api.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL
{
    internal class MediaResolvers
    {
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

        public async Task<Camera?> GetCameraAsync(
            Media media,
            CameraByIdDataLoader cameraById,
            CancellationToken cancellationToken)
        {
            if (media.CameraId.HasValue)
            {
                return await cameraById.LoadAsync(media.CameraId.Value, cancellationToken);
            }

            return null;
        }
    }
}
