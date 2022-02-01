using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Identity.Data.Mongo.Seeding;

public class DataSeeder
{
    private readonly IIdentityDbContext _identityDbContext;

    public DataSeeder(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task SeedIntialDataAsync(CancellationToken cancellationToken)
    {
        var resCount = await _identityDbContext.IdentityResources.AsQueryable()
            .CountAsync();

        var clientCount = await _identityDbContext.Clients.AsQueryable()
            .CountAsync();

        var apiResCount = await _identityDbContext.ApiResources.AsQueryable()
            .CountAsync();

        var apiScopeCount = await _identityDbContext.ApiScopes.AsQueryable()
            .CountAsync();

        if (resCount == 0)
        {
            await AddIdentityResourcesAsync(InitialData.IdentityResources, cancellationToken);
        }

        if (clientCount == 0)
        {
            await AddClientsAsync(InitialData.Clients, cancellationToken);
        }

        if (apiResCount == 0)
        {
            await AddApiResources(InitialData.ApiResources, cancellationToken);
        }

        if (apiScopeCount == 0)
        {
            await AddApiScopes(InitialData.ApiScopes, cancellationToken);
        }
    }

    public async Task AddIdentityResourcesAsync(
        IEnumerable<MagicIdentityResource> resources,
        CancellationToken cancellationToken)
    {

        await _identityDbContext.IdentityResources.InsertManyAsync(
            resources,
            options: null,
            cancellationToken);
    }

    public async Task AddClientsAsync(
        IEnumerable<MagicClient> clients,
        CancellationToken cancellationToken)
    {
        await _identityDbContext.Clients.InsertManyAsync(
            clients,
            options: null,
            cancellationToken);
    }

    public async Task AddApiResources(
        IEnumerable<MagicApiResource> resources,
        CancellationToken cancellationToken)
    {
        await _identityDbContext.ApiResources.InsertManyAsync(
            resources,
            options: null,
            cancellationToken);
    }

    public async Task AddApiScopes(
        IEnumerable<MagicApiScope> scopes,
        CancellationToken cancellationToken)
    {
        await _identityDbContext.ApiScopes.InsertManyAsync(
            scopes,
            options: null,
            cancellationToken);
    }
}
