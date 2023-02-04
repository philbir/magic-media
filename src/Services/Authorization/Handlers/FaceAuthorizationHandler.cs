using System.Threading.Tasks;
using MagicMedia.Audit;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization;

public class FaceAuthorizationHandler : AuditedAuhorizationHandler<AuhorizedOnFaceRequirement>
{
    public FaceAuthorizationHandler(IUserContextFactory userContextFactory, IAuditService auditService)
        : base(userContextFactory, auditService)
    {
        AllPermission = Permissions.Face.ViewAll;
        ResourceType = ProtectedResourceType.Face;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuhorizedOnFaceRequirement requirement)
    {
        await base.HandleRequirementAsync(context, requirement);
    }
}

