using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

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
