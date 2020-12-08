using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<string>? GetFolders(
            Album album)
        {
            return album.Includes?
                .FirstOrDefault(x => x.Type == AlbumIncludeType.Folder)?
                .Folders;
        }

        public async Task<IEnumerable<Guid>?> GetAllMediaIdsAsync(
            Album album,
            CancellationToken cancellationToken)
        {
            return await _albumService.GetMediaIdsAsync(album, cancellationToken);
        }
    }
}
