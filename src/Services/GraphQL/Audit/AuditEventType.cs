using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class AuditEventType : ObjectType<AuditEvent>
    {
        protected override void Configure(IObjectTypeDescriptor<AuditEvent> descriptor)
        {
            descriptor
                .Field("user")
                .ResolveWith<AuditResolvers>(x => x.GetUserAsync(default!, default!, default!));

            descriptor
                .Field("thumbnail")
                .ResolveWith<AuditResolvers>(x => x.GetThumbnail(default!));

            descriptor
                .Field("media")
                .ResolveWith<AuditResolvers>(x => x.GetMediaAsyc(default!, default!, default!));
        }
    }
}
