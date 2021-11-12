using Duende.IdentityServer.Models;
using MongoDB.Driver;

namespace MagicMedia.Identity.Data
{
    public interface IIdentityDbContext
    {
        IMongoCollection<MagicClient> Clients { get; }
        IMongoCollection<PersistedGrant> PersistedGrants { get; }
        IMongoCollection<MagicApiScope> ApiScopes { get; }
        IMongoCollection<MagicIdentityResource> IdentityResources { get; }
        IMongoCollection<MagicApiResource> ApiResources { get; }
        IMongoCollection<SignUpSession> SignUpSessions { get; }
        IMongoCollection<User> Users { get; }
        IMongoCollection<Invite> Invites { get; }
    }
}
