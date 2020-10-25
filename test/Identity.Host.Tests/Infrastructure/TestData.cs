using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Host.Tests.Infrastructure
{
    public class TestData
    {
        public static MagicClient Client01CredentialClient =>
            new MagicClient
            {
                ClientName = "CC01",
                ClientId = "Test.CC01",
                RequireClientSecret = true,
                ClientSecrets = new List<Secret>
                        { new Secret("secret".ToSha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowOfflineAccess = false,
                AllowedScopes = new List<string>
                {
                    "api.magic.read"
                }
            };
    }
}
