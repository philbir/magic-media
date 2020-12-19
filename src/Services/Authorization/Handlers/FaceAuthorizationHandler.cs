using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace MagicMedia.Authorization
{
    public class FaceAuthorizationHandler : AuthorizationHandler<AuhorizedOnFaceRequirement>
    {
        private readonly IUserContextFactory _userContextFactory;

        public FaceAuthorizationHandler(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuhorizedOnFaceRequirement requirement)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(
                context.User,
                CancellationToken.None);

            if (userContext.HasPermission(Permissions.Face.ViewAll))
            {
                context.Succeed(requirement);
            }
            else
            {
                try
                {
                    Guid? faceId = AuthorizationResourceIdResolver.TryGetId(context.Resource);

                    if (faceId.HasValue)
                    {
                        bool isAuthorized = await userContext.IsAuthorizedAsync(
                            faceId,
                            ProtectedResourceType.Face,
                            CancellationToken.None);

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
