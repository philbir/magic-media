using HotChocolate.AspNetCore.Authorization;
using MagicMedia.Authorization;
using MagicMedia.Store;

namespace MagicMedia.GraphQL;

[Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.AlbumEdit)]
[ExtendObjectType(RootTypes.Mutation)]
public class AlbumMutations
{
    private readonly IAlbumService _albumService;

    public AlbumMutations(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    public async Task<AddItemToAlbumPayload> AddItemsToAlbumAsync(
        AddItemToAlbumRequest input,
        CancellationToken cancellationToken)
    {
        Album album = await _albumService.AddItemsToAlbumAsync(input, cancellationToken);

        return new AddItemToAlbumPayload(album);
    }

    public async Task<UpdateAlbumPayload> RemoveFoldersFromAlbumAsync(
        RemoveFoldersFromAlbumRequest input,
        CancellationToken cancellationToken)
    {
        Album album = await _albumService.RemoveFoldersAsync(input, cancellationToken);

        return new UpdateAlbumPayload(album);
    }

    public async Task<UpdateAlbumPayload> UpdateAlbumAsync(
        UpdateAlbumRequest input,
        CancellationToken cancellationToken)
    {
        Album album = await _albumService.UpdateAlbumAsync(input, cancellationToken);
        return new UpdateAlbumPayload(album);
    }

    [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.AlbumDelete)]
    [GraphQLName("Album_Delete")]
    public async Task<DeleteAlbumPayload> DeleteAlbumAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _albumService.DeleteAsync(id, cancellationToken);

        return new DeleteAlbumPayload(id);
    }

    [GraphQLName("Album_SetCover")]
    public async Task<UpdateAlbumPayload> SetCoverAsync(
        SetAlbumCoverRequest input,
        CancellationToken cancellationToken)
    {
        Album album = await _albumService.SetCoverAsync(input, cancellationToken);

        return new UpdateAlbumPayload(album);
    }
}
