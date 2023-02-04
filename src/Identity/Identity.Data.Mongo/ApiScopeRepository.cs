using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Identity.Data.Mongo;

public class ApiScopeRepository : IApiScopeRepository
{
    private readonly IIdentityDbContext _loginDbContext;

    public ApiScopeRepository(IIdentityDbContext loginDbContext)
    {
        _loginDbContext = loginDbContext;
    }

    public async Task<IEnumerable<ApiScope>> GetByNameAsync(
        IEnumerable<string> scopeNames,
        CancellationToken cancellationToken)
    {
        List<MagicApiScope>? scopes = await _loginDbContext.ApiScopes
            .AsQueryable()
            .Where(x => scopeNames.Contains(x.Name))
            .ToListAsync(cancellationToken);

        return scopes;
    }

    public async Task<IEnumerable<ApiScope>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        List<MagicApiScope>? scopes = await _loginDbContext.ApiScopes
            .AsQueryable()
            .ToListAsync(cancellationToken);

        return scopes;
    }
}

