using System.Threading.Tasks;
using MagicMedia.Audit;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization;

public class MediaAuthorizationHandler : AuditedAuhorizationHandler<AuhorizedOnMediaRequirement>
{
    public MediaAuthorizationHandler(IUserContextFactory userContextFactory, IAuditService auditService)
        : base(userContextFactory, auditService)
    {
        AllPermission = Permissions.Media.ViewAll;
        ResourceType = ProtectedResourceType.Media;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuhorizedOnMediaRequirement requirement)
    {
        await base.HandleRequirementAsync(context, requirement);
    }
}
