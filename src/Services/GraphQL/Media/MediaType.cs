using MagicMedia.Face;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public partial class MediaType : ObjectType<Media>
    {
        protected override void Configure(IObjectTypeDescriptor<Media> descriptor)
        {
            descriptor.Ignore(x => x.Thumbnails);

            descriptor.Field("thumbnail")
                .Argument("size", a => a
                    .DefaultValue(ThumbnailSizeName.M)
                    .Type(typeof(ThumbnailSizeName)))
                .Argument("loadData", a => a
                    .DefaultValue(false)
                    .Type(typeof(bool)))
                .ResolveWith<ThumbnailResolvers>(x => x
                    .GetThumbnailAsync(default!, default!, default!, default!, default!, default!));
            //.Use(next => async context => 
            //{
            //   await next(context);
            //   context.SetScopedValue("mediaId", context.Parent<Media>().Id);
            //});

            descriptor
                .Field("camera")
                .ResolveWith<Resolvers>(x => x.GetCameraAsync(default!, default!, default!));

            descriptor
                .Field("faces")
                .ResolveWith<Resolvers>(x => x.GetFacesByMediaAsync(default!, default!, default!));

            descriptor
                .Field("files")
                .ResolveWith<Resolvers>(x => x.GetFileInfos(default!, default!));

            descriptor
                .Field("ai")
                .Argument("minConfidence", a => a
                    .DefaultValue(0.0)
                    .Type(typeof(double)))
               .ResolveWith<Resolvers>(x => x.GetAIDataAsync(default!, default!, default!, default!));
        }

        class Resolvers
        {
            public async Task<Camera?> GetCameraAsync(
                [Parent] Media media,
                [DataLoader] CameraByIdDataLoader cameraById,
                CancellationToken cancellationToken)
            {
                if (media.CameraId.HasValue)
                {
                    return await cameraById.LoadAsync(media.CameraId.Value, cancellationToken);
                }

                return null;
            }

            public Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
                [Service] IFaceService faceService,
                [Parent] Media media,
                CancellationToken cancellationToken)
                    => faceService.GetFacesByMediaAsync(media.Id, cancellationToken);

            public IEnumerable<MediaFileInfo> GetFileInfos(
                [Service] IMediaService mediaService,
                [Parent] Media media)
                    => mediaService.GetMediaFiles(media);


            public async Task<MediaAI> GetAIDataAsync(
                [Service] IMediaService mediaService,
                [Parent] Media media,
                double? minConfidence = null,
                //AISource? source=null,
                CancellationToken cancellationToken = default)
            {
                MediaAI ai = await mediaService.GetAIDataAsync(media.Id, cancellationToken);

                if (ai == null)
                {
                    return new MediaAI
                    {
                        Id = Guid.Empty,
                        MediaId = media.Id,
                        Objects = new List<MediaAIObject>(),
                        Tags = new List<MediaAITag>()
                    };
                }

                if (minConfidence.HasValue)
                {
                    ai.Objects = ai.Objects.Where(x => x.Confidence >= minConfidence);
                    ai.Tags = ai.Tags.Where(x => x.Confidence >= minConfidence);
                }

                //if (source.HasValue)
                //{
                //    ai.Objects = ai.Objects.Where(x => x.Source == source);
                //    ai.Tags = ai.Tags.Where(x => x.Source == source);
                //}

                ai.Objects = ai.Objects.OrderByDescending(x => x.Confidence);
                ai.Tags = ai.Tags.OrderByDescending(x => x.Confidence);

                return ai;
            }
        }
    }
}
