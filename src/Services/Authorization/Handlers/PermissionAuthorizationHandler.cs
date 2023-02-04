using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Audit;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<HasPermissionRequirement>
{
    private readonly IUserContextFactory _userContextFactory;
    private readonly IAuditService _auditService;

    public PermissionAuthorizationHandler(IUserContextFactory userContextFactory, IAuditService auditService)
    {
        _userContextFactory = userContextFactory;
        _auditService = auditService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasPermissionRequirement requirement)
    {
        IUserContext userContext = await _userContextFactory.CreateAsync(CancellationToken.None);

        ResourceInfo? resourceInfo = AuthorizationResourceResolver.GetResourceInfo(context.Resource);
        var auditRequest = new LogAuditEventRequest
        {
            Resource = new Store.AuditResource
            {
                Id = resourceInfo.Id?.ToString(),
                Raw = resourceInfo.Raw,
                Type = resourceInfo.Type.GetValueOrDefault()
            }
        };

        if (userContext.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
            auditRequest.GrantBy = requirement.Permission;
        }
        else
        {
            context.Fail();
        }

        auditRequest.Success = context.HasSucceeded;

        await _auditService.LogEventAsync(auditRequest, userContext!, CancellationToken.None);
    }
}
