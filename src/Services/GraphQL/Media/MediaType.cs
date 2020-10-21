using System.Threading;
using System.Threading.Tasks;
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
}
