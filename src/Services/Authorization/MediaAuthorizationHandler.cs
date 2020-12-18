using System;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace MagicMedia.Authorization
{

    public class MediaAuthorizationHandler : AuthorizationHandler<AuhorizedOnMediaRequirement>
    {
        private readonly IUserContextFactory _userContextFactory;

        public MediaAuthorizationHandler(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuhorizedOnMediaRequirement requirement)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(context.User, default);

            if (userContext.HasPermission(Permissions.Media.ViewAll))
            {
                context.Succeed(requirement);
            }
            else
            {
                try
                {
                    Guid? mediaId = AuthorizationResourceIdResolver.TryGetId(context.Resource);

                    if (mediaId.HasValue)
                    {
                        bool isAuthorized = await userContext.IsAuthorizedAsync(mediaId, ProtectedResourceType.Media, default);

                        if (isAuthorized)
                        {
                            context.Succeed(requirement);

                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in HandleRequirementAsync for Media");
                    context.Fail();
                }

                context.Fail();
            }
        }
    }
}
