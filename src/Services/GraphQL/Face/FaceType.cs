using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    public class FaceType : ObjectType<MediaFace>
    {
        protected override void Configure(IObjectTypeDescriptor<MediaFace> descriptor)
        {
            descriptor
                .Field("person")
                .ResolveWith<FaceResolvers>(x => x.GetPersonAsync(default!, default!, default!));

            descriptor
                .Field("thumbnail")
                .ResolveWith<FaceResolvers>(x => x.GetThumbnail(default!));

            descriptor
                .Field("media")
                .ResolveWith<FaceResolvers>(x => x.GetMediaAsync(default!, default!, default!));
        }
    }
}
