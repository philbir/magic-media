using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb;

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

    public async Task<IEnumerable<Album>> GetSharedWithUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        List<Album> albums = await _mediaStoreContext.Albums.AsQueryable()
            .Where(x => x.SharedWith.Contains(userId))
            .ToListAsync(cancellationToken);

        return albums;
    }

    public async Task<IEnumerable<Album>> GetWithPersonAsync(
        Guid personId,
        CancellationToken cancellationToken)
    {
        List<Album> albums = await _mediaStoreContext.Albums.AsQueryable()
            .Where(x => x.Persons.Any(p => p.PersonId == personId))
            .ToListAsync(cancellationToken);

        return albums;
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

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediaStoreContext.Albums.DeleteOneAsync(
            x => x.Id == id,
            DefaultMongoOptions.Delete,
            cancellationToken);
    }

    public async Task<SearchResult<Album>> SearchAsync(
        SearchAlbumRequest request,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Album> filter = Builders<Album>.Filter.Empty;

        if (request.SharedWithUserId.HasValue)
        {
            filter &= Builders<Album>.Filter.AnyEq(
                x => x.SharedWith, request.SharedWithUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            filter &= Builders<Album>.Filter.Regex(
                x => x.Title,
                new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*", "i"));
        }

        if (request.Persons is { } persons && persons.Any())
        {
            FilterDefinition<AlbumPerson> personFilter = Builders<AlbumPerson>.Filter
                .In(x => x.PersonId, persons);

            filter &= Builders<Album>.Filter.ElemMatch(x => x.Persons, personFilter);
        }

        IFindFluent<Album, Album>? cursor = _mediaStoreContext.Albums.Find(filter);
        long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

        List<Album> medias = await cursor
            .SortByDescending(x => x.StartDate)
            .Skip(request.PageNr * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync();

        return new SearchResult<Album>(medias, (int)totalCount);
    }

    public Task<IEnumerable<Album>> GetSharedWithAlbumsAsync(Guid userId, CancellationToken cancellationToken)
    {
        IEnumerable<Album> list = Array.Empty<Album>();

        return Task.FromResult(list);
    }
}
