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
                .ResolveWith<ThumbnailResolvers>(x => x
                    .GetThumbnailAsync(default!, default!, default!, default!));

            descriptor
                .Field("camera")
                .ResolveWith<MediaResolvers>(x => x.GetCameraAsync(default!, default!, default!));

            descriptor
                .Field("faces")
                .ResolveWith<MediaResolvers>(x => x.GetFacesByMediaAsync(default!, default!));
        }
    }

    public partial class VideoInfoType : ObjectType<VideoInfo>
    {
        protected override void Configure(IObjectTypeDescriptor<VideoInfo> descriptor)
        {
            descriptor
                .Field("duration")
                .Argument("format", a => a
                    .DefaultValue(@"mm\:ss")
                    .Type(typeof(string)))
                .Type<StringType>()
                .Resolve(c =>
                {
                    VideoInfo? info = c.Parent<VideoInfo>();

                    return info?.Duration.ToString(c.Argument<string>("format"));
                });
        }
    }
}
