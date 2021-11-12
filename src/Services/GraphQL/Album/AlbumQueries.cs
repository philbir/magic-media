using HotChocolate.AspNetCore.Authorization;
using MagicMedia.Authorization;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class AlbumQueries
    {
        public Task<IEnumerable<Album>> GetAllAlbumsAsync(
            [Service] IAlbumService albumService,
            CancellationToken cancellationToken)
                => albumService.GetAllAsync(cancellationToken);


        public Task<SearchResult<Album>> SearchAlbumsAsync(
            [Service] IAlbumService albumService,
            SearchAlbumRequest input,
            CancellationToken cancellationToken)
                => albumService.SearchAsync(input, cancellationToken);


        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.AlbumView)]
        public async Task<Album> GetAlbumAsync(
            [Service] IAlbumService albumService,
            Guid id,
            CancellationToken cancellationToken)
        {
            Album album = await albumService.GetByIdAsync(id, cancellationToken);
            album.SharedWith = album.SharedWith ?? Array.Empty<Guid>();

            return album;
        }
    }
}
