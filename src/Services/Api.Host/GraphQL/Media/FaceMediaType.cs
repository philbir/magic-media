using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL
{
    public class FaceMediaType : ObjectType<MediaFace>
    {
        protected override void Configure(IObjectTypeDescriptor<MediaFace> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
