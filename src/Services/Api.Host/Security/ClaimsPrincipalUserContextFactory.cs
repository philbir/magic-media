using System.Security.Claims;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.Api.Security;

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

    public async Task<IUserContext> CreateAsync(
        ClaimsPrincipal? principal,
        CancellationToken cancellationToken)
    {
        if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
        {
            var subject = principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            if (subject == null)
            {
                throw new ApplicationException("No sub claim found");
            }

            User? user = await _userService.GetByIdAsync(
                Guid.Parse(subject),
                cancellationToken);

            if (user != null)
            {
                return new DefaultUserContext(
                    user,
                    _userService,
                    new HttpContextClientInfoResolver(_httpContextAccessor.HttpContext!));
            }
        }

        return new NotAuthenticatedUserContext();
    }
}
