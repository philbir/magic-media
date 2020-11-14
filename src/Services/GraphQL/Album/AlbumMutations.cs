using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public class AlbumMutations
    {
        private readonly IAlbumService _albumService;

        public AlbumMutations(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        public async Task<AddItemToAlbumPayload> AddItemsToAlbumAsync(AddItemToAlbumRequest input, CancellationToken cancellationToken)
        {
            Album album = await _albumService.AddItemsToAlbumAsync(input, cancellationToken);

            return new AddItemToAlbumPayload(album);
        }
    }
}
