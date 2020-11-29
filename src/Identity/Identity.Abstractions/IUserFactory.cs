using System.Collections.Generic;
using System.Security.Claims;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services
{
    public interface IUserFactory
    {
        User CreateFromExternalLogin(string provider, string userIdentifier, IEnumerable<Claim> claims);
    }

    public record AuthenticateExternalUserRequest(
        string Provider,
        string UserIdentifier,
        IEnumerable<Claim> Claims);

    public record AuthenticateUserResult(bool Success)
    {
        public User? User { get; init; }
    }
}
