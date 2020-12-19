using System;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace MagicMedia.Authorization
{
    public class AlbumAuthorizationHandler : AuthorizationHandler<AuhorizedOnAlbumRequirement>
    {
        private readonly IUserContextFactory _userContextFactory;

        public AlbumAuthorizationHandler(IUserContextFactory userContextFactory)
        {
            _userContextFactory = userContextFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuhorizedOnAlbumRequirement requirement)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(context.User, default);

            if (userContext.HasPermission(Permissions.Album.ViewAll))
            {
                context.Succeed(requirement);
            }
            else
            {
                try
                {
                    Guid? albumId = AuthorizationResourceIdResolver.TryGetId(context.Resource);

                    if (albumId.HasValue)
                    {
                        bool isAuthorized = await userContext.IsAuthorizedAsync(albumId, ProtectedResourceType.Album, default);

                        if (isAuthorized)
                        {
                            context.Succeed(requirement);

                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in HandleRequirementAsync for Album");
                    context.Fail();
                }

                context.Fail();
            }
        }
    }
}
