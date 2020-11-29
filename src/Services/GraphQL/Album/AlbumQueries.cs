using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class AlbumQueries
    {
        private readonly IAlbumService _albumService;

        public AlbumQueries(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync(CancellationToken cancellationToken)
        {
            return await _albumService.GetAllAsync(cancellationToken);
        }

        public async Task<SearchResult<Album>> SearchAlbumsAsync(
            SearchAlbumRequest input,
            CancellationToken cancellationToken)
        {
            return await _albumService.SearchAsync(input, cancellationToken);
        }
    }
}
