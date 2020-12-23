using System;
using HotChocolate.Types;
using MagicMedia.Extensions;
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
                .Resolve(ctx =>
               {
                   MediaThumbnail thumbnail = ctx.Parent<MediaThumbnail>();

                   if (thumbnail.Data != null)
                   {
                       return thumbnail.Data.ToDataUrl(thumbnail.Format);
                   }
                   else
                   {
                       return $"api/media/{Guid.Empty}/thumbnailbyid/{thumbnail.Id}";
                   }
               });
                //.ResolveWith<ThumbnailResolvers>(x => x.GetDataUrl(default!, default!));
        }
    }
}
