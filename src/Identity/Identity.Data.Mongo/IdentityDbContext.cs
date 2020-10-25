using IdentityServer4.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo
{
    public class IdentityDbContext : MongoDbContext, IIdentityDbContext
    {
        public IdentityDbContext(MongoOptions mongoOptions)
            : base(mongoOptions, enableAutoInitialize: true)
        { }

        private IMongoCollection<MagicClient>? _clients = null;
        private IMongoCollection<PersistedGrant>? _grants = null;
        private IMongoCollection<MagicApiScope>? _apiScopes = null;
        private IMongoCollection<MagicIdentityResource>? _identityResources = null;
        private IMongoCollection<MagicApiResource>? _apiResources = null;

        public IMongoCollection<MagicClient> Clients
        {
            get
            {
                if (_clients == null)
                    _clients = CreateCollection<MagicClient>();
                return _clients;
            }
        }

        public IMongoCollection<PersistedGrant> PersistedGrants
        {
            get
            {
                if (_grants == null)
                    _grants = CreateCollection<PersistedGrant>();
                return _grants;
            }
        }

        public IMongoCollection<MagicApiScope> ApiScopes
        {
            get
            {
                if (_apiScopes == null)
                    _apiScopes = CreateCollection<MagicApiScope>();
                return _apiScopes;
            }
        }

        public IMongoCollection<MagicIdentityResource> IdentityResources
        {
            get
            {
                if (_identityResources == null)
                    _identityResources = CreateCollection<MagicIdentityResource>();
                return _identityResources;
            }
        }

        public IMongoCollection<MagicApiResource> ApiResources
        {
            get
            {
                if (_apiResources == null)
                    _apiResources = CreateCollection<MagicApiResource>();
                return _apiResources;
            }
        }

        protected override void OnConfiguring(IMongoDatabaseBuilder mongoDatabaseBuilder)
        {
            mongoDatabaseBuilder
                    .RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String))
                    .ConfigureConnection(con => con.ReadConcern = ReadConcern.Majority)
                    .ConfigureConnection(con => con.WriteConcern = WriteConcern.WMajority)
                    .ConfigureConnection(con => con.ReadPreference = ReadPreference.Primary)
                    .ConfigureCollection(new ApiScopeCollectionConfiguration())
                    .ConfigureCollection(new PersistedGrantCollectionConfiguration())
                    .ConfigureCollection(new IdentityResourceCollectionConfiguration())
                    .ConfigureCollection(new ApiResourceCollectionConfiguration())
                    .ConfigureCollection(new OneLoginClientCollectionConfiguration());
        }
    }
}
