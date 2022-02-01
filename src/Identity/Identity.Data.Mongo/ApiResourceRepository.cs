using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Identity.Data.Mongo;

public class ApiResourceRepository : IApiResourceRepository
{
    private readonly IIdentityDbContext _loginDbContext;

    public ApiResourceRepository(IIdentityDbContext loginDbContext)
    {
        _loginDbContext = loginDbContext;
    }

    public async Task<IEnumerable<MagicApiResource>> GetByNameAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken)
    {
        List<MagicApiResource>? resources = await _loginDbContext.ApiResources
            .AsQueryable()
            .Where(x => names.Contains(x.Name))
            .ToListAsync(cancellationToken);

        return resources;
    }

    public async Task<IEnumerable<MagicApiResource>> GetByScopeNameAsync(
        IEnumerable<string> scopeNames,
        CancellationToken cancellationToken)
    {
        List<MagicApiResource>? resources = await _loginDbContext.ApiResources
            .AsQueryable()
            .Where(api => api.Scopes.Any(x => scopeNames.Contains(x)))
            .ToListAsync(cancellationToken);

        return resources;
    }

    public async Task<IEnumerable<MagicApiResource>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        List<MagicApiResource>? resources = await _loginDbContext.ApiResources
            .AsQueryable()
            .ToListAsync(cancellationToken);

        return resources;
    }
}
