using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MagicMedia.Identity.Host.Tests
{
    public class JwtTokenAnalyzer
    {
        private readonly string[] _protocolClaims = new string[]
        { "exp", "nbf", "iat", "jti" };

        public JwtTokenAnalyzer(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            Token = handler.ReadToken(token) as JwtSecurityToken;
        }

        public IEnumerable<Claim> AnalyzableClaims =>
            Token?.Claims.Where(x => !_protocolClaims.Contains(x.Type));

        public JwtSecurityToken? Token { get; private set; }
    }
}
