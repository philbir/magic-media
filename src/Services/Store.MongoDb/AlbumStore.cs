using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Bson;
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

        public async Task<Album> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Albums.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Albums.AsQueryable()
                .ToListAsync(cancellationToken);
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

        public async Task<SearchResult<Album>> SearchAsync(
            SearchAlbumRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Album> filter = Builders<Album>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                filter &= Builders<Album>.Filter.Regex(
                    x => x.Title,
                    new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*" , "i"));
            }

            IFindFluent<Album, Album>? cursor = _mediaStoreContext.Albums.Find(filter);
            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<Album> medias = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<Album>(medias, (int)totalCount);
        }
    }
}
