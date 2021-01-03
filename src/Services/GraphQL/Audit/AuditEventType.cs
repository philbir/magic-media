using HotChocolate.Types;
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
                .ResolveWith<AuditResolvers>(x => x.GetUserAsync(default!, default!, default!));

            descriptor
                .Field("thumbnail")
                .ResolveWith<AuditResolvers>(x => x.GetThumbnail(default!));

            descriptor
                .Field("media")
                .ResolveWith<AuditResolvers>(x => x.GetMediaAsync(default!, default!, default!));

            descriptor
                .Field("thumbprint")
                .ResolveWith<AuditResolvers>(x => x.GetThumbprintAsync(default!, default!, default!));

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
