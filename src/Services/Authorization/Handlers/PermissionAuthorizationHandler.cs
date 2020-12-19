using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

namespace MagicMedia.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly IUserContextFactory _userContextFactory;

        public PermissionAuthorizationHandler(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasPermissionRequirement requirement)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(CancellationToken.None);
            if (userContext.HasPermission(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
