using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class AlbumType : ObjectType<Album>
    {
        protected override void Configure(IObjectTypeDescriptor<Album> descriptor)
        {
            descriptor
                .Field("thumbnail")
                .Argument("size", a => a
                    .DefaultValue(ThumbnailSizeName.M)
                    .Type(typeof(ThumbnailSizeName)))
                .ResolveWith<AlbumResolvers>(_ => _.GetThumbnailAsync(default!, default!, default!));
        }
    }
}
