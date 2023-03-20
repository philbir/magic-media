using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb;

public class TagDefinitionStore : ITagDefinitionStore
{
    private readonly MediaStoreContext _mediaStoreContext;

    public TagDefinitionStore(MediaStoreContext mediaStoreContext)
    {
        _mediaStoreContext = mediaStoreContext;
    }

    public async Task<IReadOnlyList<TagDefintion>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _mediaStoreContext.TagDefinition.AsQueryable()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TagDefintion>> GetManyAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await _mediaStoreContext.TagDefinition.AsQueryable()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
