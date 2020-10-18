using HotChocolate.Types;
using MagicMedia.Extensions;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL
{
    public class ThumbnailType : ObjectType<MediaThumbnail>
    {
        protected override void Configure(IObjectTypeDescriptor<MediaThumbnail> descriptor)
        {
            descriptor.Ignore(x => x.Data);

            descriptor
                .Field("dataUrl")
                .Type(typeof(string))
                .Resolve(x =>
                {
                    MediaThumbnail thumb = x.Parent<MediaThumbnail>();
                    return thumb.Data.ToDataUrl(thumb.Format);
                });
        }
    }
}
