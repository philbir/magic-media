using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;

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
            catch (Exception ex)
            {
                context.Fail();
            }
        }
    }

    public class AuhorizedOnMediaRequirement : IAuthorizationRequirement { }
}
