using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class AlbumStore : IAlbumStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public AlbumStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<Album> AddAsync(Album album, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Albums.InsertOneAsync(
                album,
                options: null,
                cancellationToken);

            return album;
        }

        public async Task<Album> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Albums.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Album> UpdateAsync(
            Album album,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Albums.ReplaceOneAsync(
                x => x.Id == album.Id,
                album,
                options: new ReplaceOptions(),
                cancellationToken);

            return album;
        }
    }
}
