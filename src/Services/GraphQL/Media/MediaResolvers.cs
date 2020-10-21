using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    internal class MediaResolvers
    {
        private readonly IMediaStore _mediaStore;

        public MediaResolvers(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
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

        public async Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
            Media media,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Faces.GetFacesByMediaAsync(media.Id, cancellationToken);
        }
    }
}
