using GreenDonut;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.DataLoaders;

public class TagDefinitionByIdDataLoader : BatchDataLoader<Guid, TagDefintion>
{
    private readonly ITagDefinitionStore _store;

    public TagDefinitionByIdDataLoader(
        IBatchScheduler batchScheduler,
        ITagDefinitionStore store)
        : base(batchScheduler)
    {
        _store = store;
    }

    protected override async Task<IReadOnlyDictionary<Guid, TagDefintion>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        IEnumerable<TagDefintion> cameras = await _store.GetManyAsync(keys, cancellationToken);

        return cameras.ToDictionary(x => x.Id);
    }
}
