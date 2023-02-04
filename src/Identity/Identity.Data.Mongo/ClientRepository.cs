using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MagicMedia.Identity.Data.Mongo;

public class ClientRepository : IClientRepository
{
    private readonly IIdentityDbContext _dbContext;

    public ClientRepository(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MagicClient> GetAsync(
        string id,
        CancellationToken cancellationToken)
    {
        FilterDefinition<MagicClient> findFilter =
            Builders<MagicClient>.Filter.Eq(client => client.ClientId, id);

        return (await _dbContext.Clients
                .FindAsync(findFilter, cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            .ToList()
            .SingleOrDefault();
    }

    public async Task<HashSet<string>> GetAllClientOrigins()
    {
        BsonDocument allOriginDocument = await _dbContext.Clients
            .Aggregate()
            .Project(c => new { Origin = c.AllowedCorsOrigins })
            .Unwind<BsonDocument>("Origin")
            .Group(new BsonDocument
            {
                    { "_id", new BsonDocument{{"Key", "unique" }}},
                    { "all", new BsonDocument {{ "$addToSet", "$Origin" }}}
            })
            .SingleOrDefaultAsync();

        if (allOriginDocument == null)
        {
            return new HashSet<string>();
        }

        IEnumerable<string> allOrigins = allOriginDocument
            .GetValue("all")
            .AsBsonArray
            .Select(b => b.AsString);

        var allClientOrigins = new HashSet<string>(
            allOrigins,
            StringComparer.InvariantCultureIgnoreCase);

        return allClientOrigins;
    }

    public async Task<HashSet<string>> GetAllClientRedirectUriAsync()
    {
        BsonDocument allUriDocument = await _dbContext.Clients
            .Aggregate()
            .Project(c => new { Uri = c.RedirectUris })
            .Unwind<BsonDocument>("Uri")
            .Group(new BsonDocument
            {
                    { "_id", new BsonDocument{{"Key", "unique" }}},
                    { "all", new BsonDocument {{ "$addToSet", "$Uri" } }}
            })
            .SingleOrDefaultAsync();

        if (allUriDocument == null)
        {
            return new HashSet<string>();
        }

        IEnumerable<string> allUris = allUriDocument
            .GetValue("all")
            .AsBsonArray
            .Select(b => b.AsString);

        var allClientRedirectUris = new HashSet<string>(
            allUris,
            StringComparer.InvariantCultureIgnoreCase);

        return allClientRedirectUris;
    }
}
