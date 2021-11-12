using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;
using MagicMedia.Thumbprint;

namespace MagicMedia.GraphQL
{
    public class AuditEventType : ObjectType<AuditEvent>
    {
        protected override void Configure(IObjectTypeDescriptor<AuditEvent> descriptor)
        {
            descriptor.Ignore(c => c.Client);

            descriptor
                .Field("user")
                .ResolveWith<Resolvers>(x => x.GetUserAsync(default!, default!, default!));

            descriptor
                .Field("thumbnail")
                .ResolveWith<Resolvers>(x => x.GetThumbnail(default!));

            descriptor
                .Field("media")
                .ResolveWith<Resolvers>(x => x.GetMediaAsync(default!, default!, default!));

            descriptor
                .Field("thumbprint")
                .ResolveWith<Resolvers>(x => x.GetThumbprintAsync(default!, default!, default!));

        }

        class Resolvers
        {
            public async Task<User?> GetUserAsync(
                [Parent] AuditEvent auditEvent,
                [DataLoader] UserByIdDataLoader userById,
                CancellationToken cancellationToken)
            {
                if (auditEvent.UserId.HasValue)
                {
                    return await userById.LoadAsync(auditEvent.UserId.Value, cancellationToken);
                }

                return null;
            }

            public async Task<Media?> GetMediaAsync(
                [Parent] AuditEvent auditEvent,
                [DataLoader] MediaByIdDataLoader mediaByid,
                CancellationToken cancellationToken)
            {
                if (auditEvent.Resource is { } r &&
                    r.Type == ProtectedResourceType.Media &&
                    r.Id != null)
                {
                    if (Guid.TryParse(r.Id, out Guid mediaId))
                    {
                        return await mediaByid.LoadAsync(mediaId, cancellationToken);
                    }
                }

                return null;
            }

            public async Task<ClientThumbprint?> GetThumbprintAsync(
                [Parent] AuditEvent auditEvent,
                [DataLoader] ClientThumbprintByIdDataLoader thumbprintById,
                CancellationToken cancellationToken)
            {
                if (auditEvent.ThumbprintId != null)
                {
                    return await thumbprintById.LoadAsync(auditEvent.ThumbprintId, cancellationToken);
                }

                return null;
            }

            public string? GetThumbnail(AuditEvent auditEvent)
            {
                if (auditEvent.Resource is { } r && r.Id is { })
                {
                    if (r.Type == ProtectedResourceType.Media)
                    {
                        return $"/api/media/{r.Id}/thumbnail/{ThumbnailSizeName.SqXs}";
                    }
                    else if (r.Type == ProtectedResourceType.Face)
                    {
                        return $"/api/face/{r.Id}/thumbnail";
                    }
                }

                return null;
            }
        }
    }

    public class UserAgentInfoType : ObjectType<UserAgentInfo>
    {
        private readonly IUserAgentInfoService _userAgentInfoService;

        public UserAgentInfoType(IUserAgentInfoService userAgentInfoService)
        {
            _userAgentInfoService = userAgentInfoService;
        }

        protected override void Configure(IObjectTypeDescriptor<UserAgentInfo> descriptor)
        {
            descriptor
                .Field("description")
                .Resolver(c =>
                {
                   UserAgentInfo info = c.Parent<UserAgentInfo>();

                    return info.ToShortDescription();
                });
        }
    }
}
