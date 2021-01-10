using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace MagicMedia.Identity.Data.Mongo.Seeding
{
    public class InitialData
    {
        private static readonly ICollection<Secret> DefaultSecrets = new List<Secret>
                        { new Secret("geCDNACu94a5DfZQ2Sm46DBjkSErAnNA".ToSha256()) };

        public static IEnumerable<MagicIdentityResource> IdentityResources =>
            new List<MagicIdentityResource>
            {
                new MagicIdentityResource
                {
                    Name = "openid",
                    ShowInDiscoveryDocument = true,
                    UserClaims = new List<string> { "sub" }
                },
                new MagicIdentityResource
                {
                    Name = "profile",
                    ShowInDiscoveryDocument = true,
                    UserClaims = new List<string> { "email", "given_name", "family_name" }
                }
            };

        public static IEnumerable<MagicApiResource> ApiResources =>
            new List<MagicApiResource>
            {
                new MagicApiResource
                {
                    Name = "api.magic",
                    DisplayName = "Media Magic API",
                    Scopes = new List<string>
                    {
                        "api.magic.read",
                        "api.magic.write",
                        "api.magic.imageai"
                    }
                }
            };

        public static IEnumerable<MagicApiScope> ApiScopes =>
            new List<MagicApiScope>
            {
                new MagicApiScope
                {
                    Name = "api.magic.read",
                    DisplayName = "Read media API",
                },
                new MagicApiScope
                {
                    Name = "api.magic.write",
                    DisplayName = "Write media API",
                },
                new MagicApiScope
                {
                    Name = "api.magic.imageai",
                    DisplayName = "Read an write ImageAI controller",
                },
            };

        public static IEnumerable<MagicClient> Clients =>
            new List<MagicClient>
            {
                new MagicClient
                {
                    ClientName = "Media UI",
                    ClientId = "Media.UI",
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = DefaultSecrets,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        "http://localhost:5000/signin-oidc"
                    },
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api.magic.read",
                        "api.magic.write",
                    },
                },
                new MagicClient
                {
                    ClientName = "Media Test",
                    ClientId = "Media.Test",
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = DefaultSecrets,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        "api.magic.read",
                        "api.magic.write",
                    },
                },
                new MagicClient
                {
                    ClientName = "ImageAI Job",
                    ClientId = "ImageAI.Job",
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = DefaultSecrets,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        "api.magic.imageai",
                    },
                }
            };
    }
}
