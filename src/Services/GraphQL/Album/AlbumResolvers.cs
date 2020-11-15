using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class AlbumResolvers
    {
        private readonly IAlbumService _albumService;

        public AlbumResolvers(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(
            Album album,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            return await _albumService.GetThumbnailAsync(album, size, cancellationToken);
        }
    }
}
