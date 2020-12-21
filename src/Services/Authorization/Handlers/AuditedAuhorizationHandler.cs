using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Audit;
using MagicMedia.Authorization;
using MagicMedia.Security;
using MagicMedia.Store;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace MagicMedia.Authorization
{
    public class AuditedAuhorizationHandler<TRequirement>
        : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
    {
        protected readonly IUserContextFactory _userContextFactory;
        private readonly IAuditService _auditService;

        protected LogAuditEventRequest AuditRequest = new LogAuditEventRequest();

        public ResourceInfo? ResourceInfo { get; private set; }

        protected IUserContext? UserContext { get; private set; }

        public virtual ProtectedResourceType ResourceType { get; protected set; }

        public string Action { get; protected set; } = "View";

        public string AllPermission { get; protected set; } = default!;

        public AuditedAuhorizationHandler(
            IUserContextFactory userContextFactory,
            IAuditService auditService)
        {
            _userContextFactory = userContextFactory;
            _auditService = auditService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            ResourceInfo = AuthorizationResourceResolver.GetResourceInfo(context.Resource);
            UserContext = await _userContextFactory.CreateAsync(context.User, default);

            AuditRequest = new LogAuditEventRequest
            {
                Action = Action,
                Resource = new AuditResource
                {
                    Type = ResourceType,
                    Id = ResourceInfo!.Id?.ToString(),
                    Raw = ResourceInfo.Raw
                }
            };

            if (UserContext!.HasPermission(AllPermission))
            {
                context.Succeed(requirement);
                AuditRequest.GrantBy = AllPermission;
            }
            else
            {
                try
                {
                    Guid? mediaId = (Guid)ResourceInfo!.Id;

                    if (mediaId.HasValue)
                    {
                        bool isAuthorized = await UserContext.IsAuthorizedAsync(mediaId, ResourceType, default);

                        if (isAuthorized)
                        {
                            AuditRequest.GrantBy = "OnId";
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
                    Log.Error(ex, "Error in HandleRequirementAsync for Media");
                    context.Fail();
                }
            }

            AuditRequest.Success = context.HasSucceeded;

            await SaveAuditAsync();
        }

        protected async Task SaveAuditAsync()
        {
            await _auditService.LogEventAsync(AuditRequest, UserContext!, CancellationToken.None);
        }
    }
}
