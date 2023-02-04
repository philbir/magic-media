using System.Collections.Generic;
using Duende.IdentityServer.Models;
using IdentityModel;
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
