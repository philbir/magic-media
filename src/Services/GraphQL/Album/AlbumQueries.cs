using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MagicMedia.Authorization;
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


        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.AlbumView)]
        public async Task<Album> GetAlbumAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Album album = await _albumService.GetByIdAsync(id, cancellationToken);
            album.SharedWith = album.SharedWith ?? Array.Empty<Guid>();

            return album;
        }
    }
}
