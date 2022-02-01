using MagicMedia.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequirePermission(
        this AuthorizationPolicyBuilder builder,
        string permission)
    {
        builder.AddRequirements(new HasPermissionRequirement(permission));

        return builder;
    }
}
