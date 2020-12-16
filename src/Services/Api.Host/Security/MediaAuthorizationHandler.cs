using System;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace MagicMedia.Api.Security
{
    public class MediaAuthorizationHandler :
            AuthorizationHandler<AuhorizedOnMediaRequirement, Guid>
    {
        private readonly IUserContextFactory _userContextFactory;

        public MediaAuthorizationHandler(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuhorizedOnMediaRequirement requirement,
            Guid resource)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(context.User, default);

            try
            {
                if (userContext.HasPermission(Permissions.Media.ViewAll))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    bool isAuthorized = await userContext.IsAuthorizedAsync(resource, ProtectedResourceType.Media, default);

                    if (isAuthorized)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in HandleRequirementAsync for resource: {Resource}", resource);
                context.Fail();
            }
        }
    }

    public class AuhorizedOnMediaRequirement : IAuthorizationRequirement { }
}
