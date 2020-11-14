using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;

namespace MagicMedia
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumStore _albumStore;

        public AlbumService(IAlbumStore albumStore)
        {
            _albumStore = albumStore;
        }

        public async Task<Album> AddAsync(string title, CancellationToken cancellationToken)
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = title
            };

            await _albumStore.AddAsync(album, cancellationToken);

            return album;
        }

        public async Task<Album> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _albumStore.GetById(id, cancellationToken);
        }

        public async Task<Album> AddItemsToAlbumAsync(
            AddItemToAlbumRequest request,
            CancellationToken cancellationToken)
        {
            Album? album;

            if (!request.AlbumId.HasValue)
            {
                album = await AddAsync(request.NewAlbumTitle!, cancellationToken);
            }
            else
            {
                album = await GetById(request.AlbumId.Value, cancellationToken);
            }

            AddMediaIds(request, album);

            await _albumStore.UpdateAsync(album, cancellationToken);

            return album;
        }

        private static void AddMediaIds(AddItemToAlbumRequest request, Album album)
        {
            if (request.MediaIds is { } ids && ids.Any())
            {
                AlbumInclude? idInclude = album.Includes
                    .FirstOrDefault(x => x.Type == AlbumIncludeType.Ids);

                if (idInclude == null)
                {
                    idInclude = new AlbumInclude();
                    album.Includes.Add(idInclude);
                }
                var toAdd = new HashSet<Guid>(idInclude.MediaIds);

                foreach (Guid newId in request.MediaIds)
                {
                    toAdd.Add(newId);
                }

                idInclude.MediaIds = toAdd;
            }
        }
    }
}
