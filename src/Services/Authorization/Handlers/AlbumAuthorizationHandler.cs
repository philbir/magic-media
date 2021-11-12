using System.Threading.Tasks;
using MagicMedia.Audit;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization
{
    public class AlbumAuthorizationHandler : AuditedAuhorizationHandler<AuhorizedOnAlbumRequirement>
    {
        public AlbumAuthorizationHandler(IUserContextFactory userContextFactory, IAuditService auditService)
            : base(userContextFactory, auditService)
        {
            AllPermission = Permissions.Album.ViewAll;
            ResourceType = ProtectedResourceType.Album;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuhorizedOnAlbumRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);
        }
    }
}
