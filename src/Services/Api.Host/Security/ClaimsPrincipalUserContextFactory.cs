using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;
using Microsoft.AspNetCore.Http;

namespace MagicMedia.Api.Security
{
    public class ClaimsPrincipalUserContextFactory
        : IUserContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public ClaimsPrincipalUserContextFactory(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }


        public async Task<IUserContext> CreateAsync(CancellationToken cancellationToken)
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

            return await CreateAsync(principal, cancellationToken);
        }

        public async Task<IUserContext> CreateAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken)
        {
            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
            {
                var subject = principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                User? user = await _userService.TryGetByIdAsync(Guid.Parse(subject), cancellationToken);

                if (user != null)
                {
                    return new DefaultUserContext(user);
                }
            }

            return new NotAuthenticatedUserContext();
        }
    }
}
