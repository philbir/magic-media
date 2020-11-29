using System.Collections.Generic;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MagicMedia.Identity.Data.Mongo
{
    public class IdentityResourceRepository : IIdentityResourceRepository
    {
        private readonly IIdentityDbContext _dbContext;

        public IdentityResourceRepository(IIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MagicIdentityResource>> GetByNameAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken)
        {
            List<MagicIdentityResource>? resources = await _dbContext
                    .IdentityResources.AsQueryable()
                .Where(x => names.Contains(x.Name))
                .ToListAsync(cancellationToken);

            return resources;
        }

        public async Task<IEnumerable<MagicIdentityResource>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.IdentityResources.AsQueryable()
                .ToListAsync(cancellationToken);
        }
    }
}

