using HotChocolate;
using HotChocolate.Types;
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
                    .GetThumbnailAsync(default!, default!, default!, default!, default!));
                //.Use(next => async context => 
                //{
                //   await next(context);
                //   context.SetScopedValue("mediaId", context.Parent<Media>().Id);
                //});

            descriptor
                .Field("camera")
                .ResolveWith<MediaResolvers>(x => x.GetCameraAsync(default!, default!, default!));

            descriptor
                .Field("faces")
                .ResolveWith<MediaResolvers>(x => x.GetFacesByMediaAsync(default!, default!));

            descriptor
                .Field("files")
                .ResolveWith<MediaResolvers>(x => x.GetFileInfos(default!));

            descriptor
                .Field("ai")
                .Argument("minConfidence", a => a
                    .DefaultValue(0.0)
                    .Type(typeof(double)))
               .ResolveWith<MediaResolvers>(x => x.GetAIDataAsync(default!, default!, default!));
        }
    }
}
