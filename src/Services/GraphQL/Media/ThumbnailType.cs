using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class ThumbnailType : ObjectType<MediaThumbnail>
    {
        protected override void Configure(IObjectTypeDescriptor<MediaThumbnail> descriptor)
        {
            descriptor.Ignore(x => x.Data);

            descriptor
                .Field("dataUrl")
                .Type(typeof(string))
                .ResolveWith<ThumbnailResolvers>(x => x.GetDataUrl(default!, default!));
        }
    }
}
