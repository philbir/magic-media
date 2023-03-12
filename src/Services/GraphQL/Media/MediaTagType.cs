using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL;

public class MediaTagType : ObjectType<MediaTag>
{
    protected override void Configure(IObjectTypeDescriptor<MediaTag> descriptor)
    {
        descriptor.Field("definition")
            .ResolveWith<Resolvers>(x => x.GetTagDefintionAsync(default!, default!, default!));
    }

    private class Resolvers
    {
        internal Task<TagDefintion> GetTagDefintionAsync(
            [DataLoader] TagDefinitionByIdDataLoader dataLoader,
            [Parent] MediaTag tag,
            CancellationToken cancellationToken)
        {
            return dataLoader.LoadAsync(tag.DefinitionId, cancellationToken);
        }
    }
}
