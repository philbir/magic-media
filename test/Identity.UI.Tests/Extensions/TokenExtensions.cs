using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicMedia.Identity.UI.Tests.Extensions
{
    public static class TokenExtensions
    {
        public static IEnumerable<Claim> GetClaims(this string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

            return securityToken.Claims;
        }
    }
}
