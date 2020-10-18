using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Api.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL
{
    public class MediaType : ObjectType<Media>
    {
        protected override void Configure(IObjectTypeDescriptor<Media> descriptor)
        {
            descriptor.Ignore(x => x.Thumbnails);

            descriptor.Field("thumbnail")
                .Argument("size", a => a
                    .DefaultValue(ThumbnailSizeName.M)
                    .Type(typeof(ThumbnailSizeName)))
                .ResolveWith<MediaResolvers>(x => x
                    .GetThumbnailAsync(default!, default!, default!, default!));

            descriptor
                .Field("camera")
                .ResolveWith<MediaResolvers>(x => x.GetCameraAsync(default!, default!, default!));
        }

        public class MediaResolvers
        {
            private readonly ICameraService _cameraService;

            public MediaResolvers(ICameraService cameraService)
            {
                _cameraService = cameraService;
            }

            public async Task<MediaThumbnail> GetThumbnailAsync(
                Media media,
                ThumbnailByMediaIdDataLoader thumbnailLoader,
                ThumbnailSizeName size,
                CancellationToken cancellationToken)
            {
                thumbnailLoader.Size = size;

                return await thumbnailLoader.LoadAsync(media.Id, cancellationToken);
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
}
