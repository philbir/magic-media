using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Services;

public class UserFactory : IUserFactory
{
    public User CreateFromExternalLogin(
        string provider,
        string userIdentifier,
        IEnumerable<Claim> claims)
    {
        var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Active,
            AuthProviders = new List<UserAuthProvider>
                {
                    new() { Name = provider, UserIdentifier = userIdentifier}
                }
        };
    }
}
