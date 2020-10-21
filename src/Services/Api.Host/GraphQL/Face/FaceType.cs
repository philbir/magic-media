using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL.Face
{
    public class FaceType : ObjectType<MediaFace>
    {
        protected override void Configure(IObjectTypeDescriptor<MediaFace> descriptor)
        {
            descriptor
                .Field("person")
                .ResolveWith<FaceResolvers>(x => x.GetPersonAsync(default!, default!, default!));
        }
    }
}
