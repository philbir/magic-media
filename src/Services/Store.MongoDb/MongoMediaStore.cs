using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb;

public class MongoMediaStore : IMediaStore
{
    private readonly MediaStoreContext _mediaStoreContext;

    public MongoMediaStore(
        MediaStoreContext mediaStoreContext,
        IThumbnailBlobStore thumbnailBlobStore,
        IMediaBlobStore blobStore,
        IFaceStore faceStore,
        IAlbumStore albumStore,
        ICameraStore cameraStore,
        IPersonStore personStore,
        IMediaAIStore mediaAIStore,
        IUserStore userStore,
        ITagDefinitionStore tagDefinitions)
    {
        _mediaStoreContext = mediaStoreContext;
        Thumbnails = thumbnailBlobStore;
        Blob = blobStore;
        Faces = faceStore;
        Albums = albumStore;
        Cameras = cameraStore;
        Persons = personStore;
        MediaAI = mediaAIStore;
        Users = userStore;
        TagDefinitions = tagDefinitions;
    }

    public IFaceStore Faces { get; }

    public IAlbumStore Albums { get; }

    public ICameraStore Cameras { get; }

    public IPersonStore Persons { get; }

    public IMediaAIStore MediaAI { get; }

    public IUserStore Users { get; }

    public ITagDefinitionStore TagDefinitions { get; }

    public IThumbnailBlobStore Thumbnails { get; }

    public IMediaBlobStore Blob { get; }

    public async Task<SearchResult<Media>> SearchAsync(
        SearchMediaRequest request,
        Func<Guid, CancellationToken, Task<IEnumerable<Guid>>> albumMediaResolver,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag("search:folder", request.Folder);

        FilterDefinition<Media>? filter = await BuildFilterFromRequestAsync(request, albumMediaResolver, cancellationToken);

        IFindFluent<Media, Media>? cursor = _mediaStoreContext.Medias.Find(filter);

        if (request.AlbumId.HasValue)
        {
            cursor = cursor.SortBy(x => x.DateTaken);
        }
        else
        {
            cursor = cursor.SortByDescending(x => x.DateTaken);
        }

        List<Media> medias = await cursor
            .Skip(request.PageNr * request.PageSize)
            .Limit(request.PageSize + 1)
            .ToListAsync();

        return new SearchResult<Media>(
            medias.Take(request.PageSize),
            medias.Count() > request.PageSize);
    }

    public async Task<IEnumerable<Guid>> GetIdsFromSearchRequestAsync(
        SearchMediaRequest request,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Media>? filter = await BuildFilterFromRequestAsync(
            request,
            albumMediaResolver: null,
            cancellationToken);

        ProjectionDefinition<Media> projection = Builders<Media>.Projection
            .Include(x => x.Id);

        var options = new FindOptions<Media, BsonDocument> { Projection = projection };

        IAsyncCursor<BsonDocument>? cursor = await _mediaStoreContext.Medias.FindAsync(
            filter,
            options,
            cancellationToken);

        List<BsonDocument> docs = await cursor.ToListAsync(cancellationToken);

        return docs.Select(x => x["_id"].AsGuid);
    }

    private async Task<FilterDefinition<Media>> BuildFilterFromRequestAsync(
        SearchMediaRequest request,
        Func<Guid, CancellationToken, Task<IEnumerable<Guid>>>? albumMediaResolver,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Media>? filter = await new MediaFilterBuilder(
            _mediaStoreContext,
            albumMediaResolver,
            cancellationToken)
            .AddAuthorizedOn(request.AuthorizedOnMedia)
            .AddText(request.Text)
            .AddFolder(request.Folder)
            .AddPersons(request.Persons)
            .AddGroups(request.Groups)
            .AddCities(request.Cities)
            .AddCountries(request.Countries)
            .AddMediaTypes(request.MediaTypes)
            .AddCameras(request.Cameras)
            .AddAlbum(request.AlbumId)
            .AddGeoRadius(request.GeoRadius)
            .AddDate(request.Date)
            .AddTags(request.Tags)
            .AddAITags(request.AiTags)
            .AddAIObjects(request.Objects)
            .BuildAsync();

        return filter;
    }

    public async Task<Media> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag("mediaId", id);

        Media media = await _mediaStoreContext.Medias.AsQueryable()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return media;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediaStoreContext.Medias.DeleteOneAsync(
            x => x.Id == id,
            DefaultMongoOptions.Delete,
            cancellationToken);
    }

    public async Task DeleteMediaAIAsync(Guid mediaId, CancellationToken cancellationToken)
    {
        await _mediaStoreContext.MediaAI.DeleteManyAsync(
            x => x.MediaId == mediaId,
            DefaultMongoOptions.Delete,
            cancellationToken);
    }

    public async Task<IEnumerable<Media>> GetManyAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        List<Media> medias = await _mediaStoreContext.Medias.AsQueryable()
            .Where(x => ids.ToList().Contains(x.Id))
            .ToListAsync(cancellationToken);

        return medias;
    }

    public async Task<Dictionary<Guid, IEnumerable<MediaHash>>> GetAllHashesAsync(
        CancellationToken cancellationToken)
    {
        List<Media> medias = await _mediaStoreContext.Medias.AsQueryable()
            .Where(x => x.Hashes != null)
            .Select(x => new Media
            {
                Id = x.Id,
                Hashes = x.Hashes
            })
            .ToListAsync(cancellationToken);

        return medias.ToDictionary(x => x.Id, y => y.Hashes);
    }

    public async Task<IReadOnlyList<MediaTag>> SetMediaTagAsync(
        Guid id,
        MediaTag tag,
        CancellationToken cancellationToken)
    {
        Media media = await GetByIdAsync(id, cancellationToken);
        var mediaTags = media.Tags.ToList();

        MediaTag? existing = mediaTags.SingleOrDefault(x => x.DefinitionId == tag.DefinitionId);

        if (existing is {})
        {
            existing.Data = tag.Data;
            existing.ModifiedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            mediaTags.Add(new MediaTag
            {
                DefinitionId = tag.DefinitionId,
                Data = tag.Data,
                ModifiedAt = DateTimeOffset.Now
            });
        }

        UpdateResult result = await _mediaStoreContext.Medias.UpdateOneAsync(
            x => x.Id == id,
            Builders<Media>.Update.Set(x => x.Tags, mediaTags),
            new UpdateOptions(),
            cancellationToken);

        return mediaTags;
    }

    public async Task<IReadOnlyDictionary<Guid, MediaThumbnail>> GetThumbnailsByMediaIdsAsync(
        IEnumerable<Guid> mediaIds,
        ThumbnailSizeName size,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Media> filter = Builders<Media>.Filter
            .In(x => x.Id, mediaIds.ToList());

        filter = filter & Builders<Media>.Filter.ElemMatch(x =>
            x.Thumbnails,
            Builders<MediaThumbnail>.Filter.Eq(t => t.Size, size));

        List<Media> medias = await _mediaStoreContext.Medias.Find(filter)
            .ToListAsync(cancellationToken);

        var result = new Dictionary<Guid, MediaThumbnail>();

        foreach (Media media in medias)
        {
            MediaThumbnail? thumb = media!.Thumbnails!.Where(x =>
                x.Size == size
                /*x.Format == "webp" */)
                .FirstOrDefault();

            if (thumb != null)
            {
                thumb.Data = await Thumbnails.GetAsync(thumb.Id, cancellationToken);
                result.Add(media.Id, thumb);
            }
        }

        return result;
    }

    public async Task SaveFacesAsync(
        Guid mediaId,
        IEnumerable<MediaFace> faces,
        CancellationToken cancellationToken)
    {
        if (faces != null)
        {
            foreach (MediaFace face in faces)
            {
                await Thumbnails.StoreAsync(
                    new ThumbnailData(face.Thumbnail.Id, face.Thumbnail.Data),
                    cancellationToken);

                face.Thumbnail.Data = null;
            }

            if (faces.Any())
            {
                await _mediaStoreContext.Faces.InsertManyAsync(
                    faces,
                    DefaultMongoOptions.InsertMany,
                    cancellationToken);
            }
        }

        await _mediaStoreContext.Medias.UpdateOneAsync(
            x => x.Id == mediaId,
            Builders<Media>.Update.Set(f => f.FaceCount, faces.Count()),
            DefaultMongoOptions.Update,
            cancellationToken);
    }

    public async Task UpdateAsync(Media media, CancellationToken cancellationToken)
    {
        await _mediaStoreContext.Medias.ReplaceOneAsync(
            x => x.Id == media.Id,
            media,
            options: new ReplaceOptions(),
            cancellationToken);
    }

    public async Task RemoveTagsByDefinitionIdAsync(
        Guid id,
        IEnumerable<Guid> definitionIds,
        CancellationToken cancellationToken)
    {
        UpdateDefinition<Media> update = Builders<Media>.Update.PullFilter(
            x => x.Tags,
            Builders<MediaTag>.Filter.In(x => x.DefinitionId, definitionIds));

        await _mediaStoreContext.Medias.UpdateOneAsync(
            x => x.Id == id,
            update,
            new UpdateOptions(),
            cancellationToken);
    }

    public async Task UpdateThumbnailsAsync(
        Media media,
        IEnumerable<MediaThumbnail> thumbnails,
        CancellationToken cancellationToken)
    {
        await DeleteThumbnailsByMediaAsync(media, cancellationToken);

        media.Thumbnails = thumbnails;
        await StoreThumbnailsAsync(media.Thumbnails, cancellationToken);
    }

    private async Task StoreThumbnailsAsync(
        IEnumerable<MediaThumbnail> thumbnails,
        CancellationToken cancellationToken)
    {
        foreach (MediaThumbnail thumb in thumbnails)
        {
            await Thumbnails.StoreAsync(
                new ThumbnailData(thumb.Id, thumb.Data),
                cancellationToken);

            thumb.Data = null;
        }
    }

    private async Task DeleteThumbnailsByMediaAsync(Media media, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (MediaThumbnail thumb in media.Thumbnails)
        {
            tasks.Add(DeleteAsync(thumb.Id, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    public async Task InsertMediaAsync(
        Media media,
        IEnumerable<MediaFace>? faces,
        CancellationToken cancellationToken)
    {
        await StoreThumbnailsAsync(media.Thumbnails, cancellationToken);

        await _mediaStoreContext.Medias.InsertOneAsync(
            media,
            DefaultMongoOptions.InsertOne,
            cancellationToken);

        if (faces != null)
        {
            foreach (MediaFace face in faces)
            {
                await Thumbnails.StoreAsync(
                    new ThumbnailData(face.Thumbnail.Id, face.Thumbnail.Data),
                    cancellationToken);

                face.Thumbnail.Data = null;
            }
        }

        if (faces.Any())
        {
            await _mediaStoreContext.Faces.InsertManyAsync(
                faces,
                DefaultMongoOptions.InsertMany,
                cancellationToken);
        }
    }

    public async Task<IEnumerable<MediaHeaderData>> GetHeaderDataAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await _mediaStoreContext.Medias.AsQueryable()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new MediaHeaderData(x.Id, x.Filename, x.DateTaken))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAllFoldersAsync(
        IEnumerable<Guid>? ids,
        CancellationToken cancellationToken)
    {
        IMongoQueryable<Media> query = _mediaStoreContext.Medias.AsQueryable()
            .Where(x => x.Folder != null);

        query = AddAuthorizedOnFilterAsync(query, ids);

        return await query
            .Select(x => x.Folder!)
            .Distinct()
            .ToListAsync(cancellationToken);
    }



    public IMongoQueryable<Media> AddAuthorizedOnFilterAsync(IMongoQueryable<Media> query, IEnumerable<Guid>? ids)
    {
        if (ids != null)
        {
            query = query.Where(x => ids.Contains(x.Id));
        }

        return query;
    }

    public async Task<IEnumerable<Media>> GetMediaWithoutAISourceAsync(
        AISource source,
        int limit,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Media> filter = Builders<Media>.Filter.And(
            Builders<Media>.Filter.Ne("AISummary.Sources", source),
            Builders<Media>.Filter.Eq(x => x.MediaType, MediaType.Image),
            Builders<Media>.Filter.Eq(x => x.State, MediaState.Active));

        IFindFluent<Media, Media>? cursor = _mediaStoreContext.Medias.Find(filter);

        return await cursor.Limit(limit).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SearchFacetItem>> GetGroupedCitiesAsync(
        IEnumerable<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        BsonDocument? idMatch = AggregationPipelineFactory.CreateMatchInStage(mediaIds);

        IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
            CollectionNames.Media,
            "Media_GroupByCity",
            idMatch,
            cancellationToken);

        var result = new List<SearchFacetItem>();

        foreach (BsonDocument doc in docs)
        {
            var item = new SearchFacetItem();
            item.Count = doc["count"].AsInt32;

            if (doc["_id"].IsString)
            {
                item.Text = doc["_id"].AsString;
                item.Value = doc["_id"].AsString;
                result.Add(item);
            }
        }

        return result.OrderByDescending(x => x.Count);
    }


    public async Task<IEnumerable<SearchFacetItem>> GetGroupedCamerasAsync(
        IEnumerable<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        BsonDocument? idMatch = AggregationPipelineFactory.CreateMatchInStage(mediaIds);

        IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
            CollectionNames.Media,
            "Media_GroupByCamera",
            idMatch,
            cancellationToken);

        var result = new List<SearchFacetItem>();

        foreach (BsonDocument doc in docs)
        {
            try
            {
                var item = new SearchFacetItem();
                item.Count = doc["Count"].AsInt32;

                if (doc["Text"] != BsonNull.Value)
                {
                    item.Text = doc["Text"].AsString;
                    item.Value = doc["Value"].AsGuid.ToString("N");
                    result.Add(item);
                }
            }
            catch { }
        }

        return result.OrderByDescending(x => x.Count);
    }


    public async Task<IEnumerable<Guid>> GetIdsByFolderAsync(
        string folder,
        CancellationToken cancellationToken)
    {
        FilterDefinition<Media> filter = Builders<Media>.Filter.Regex(
            x => x.Folder,
            new BsonRegularExpression("^" + Regex.Escape(folder), "i"));

        ProjectionDefinition<Media> projection = Builders<Media>.Projection
            .Include(x => x.Id);

        var options = new FindOptions<Media, BsonDocument> { Projection = projection };

        IAsyncCursor<BsonDocument> cursor = await _mediaStoreContext.Medias.FindAsync(filter, options, cancellationToken);
        List<BsonDocument> docs = await cursor.ToListAsync(cancellationToken);

        return docs.Select(x => x["_id"].AsGuid);
    }

    public async Task<IEnumerable<SearchFacetItem>> GetGroupedCountriesAsync(
        IEnumerable<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        BsonDocument? idMatch = AggregationPipelineFactory.CreateMatchInStage(mediaIds);

        IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
            CollectionNames.Media,
            "Media_GroupByCountry",
            idMatch,
            cancellationToken);

        var result = new List<SearchFacetItem>();

        foreach (BsonDocument doc in docs)
        {
            var item = new SearchFacetItem();
            item.Count = doc["count"].AsInt32;

            var idDoc = doc["_id"] as BsonDocument;

            if (idDoc.Contains("code"))
            {
                if (idDoc["code"].IsString)
                {
                    item.Value = idDoc["code"].AsString;
                    item.Text = idDoc["name"].AsString;
                }
                else
                {
                    item.Value = "Empty";
                    item.Text = "Empty";
                }

                result.Add(item);
            }
        }

        return result.OrderByDescending(x => x.Count);
    }

    public async Task UpdateAISummaryAsync(
        Guid mediaId,
        MediaAISummary mediaAISummary,
        CancellationToken cancellationToken)
    {
        UpdateDefinition<Media> update = Builders<Media>.Update.Set(x => x.AISummary, mediaAISummary);

        await _mediaStoreContext.Medias.UpdateOneAsync(
            x => x.Id == mediaId,
            update,
            DefaultMongoOptions.Update,
            cancellationToken);
    }


    public async Task<IEnumerable<MediaGeoLocation>> FindMediaInGeoBoxAsync(
        GeoBox box,
        IEnumerable<Guid>? mediaIds,
        int limit,
        CancellationToken cancellation)
    {
        FilterDefinition<Media> filter = Builders<Media>.Filter.GeoWithinBox(x =>
           x.GeoLocation.Point,
           box.SouthWest.Longitude,
           box.SouthWest.Latitude,
           box.NorthEast.Longitude,
           box.NorthEast.Latitude);

        if (mediaIds != null)
        {
            filter &= Builders<Media>.Filter.In(x => x.Id, mediaIds);
        }

        var medias = await _mediaStoreContext.Medias.Find(filter)
            .Limit(limit)
            .Project(p => new
            {
                p.Id,
                p.GeoLocation!.Point.Coordinates,
                p.GeoLocation!.GeoHash
            })
            .ToListAsync(cancellation);

        IEnumerable<MediaGeoLocation> locs = medias.Select(x => new MediaGeoLocation
        {
            Id = x.Id,
            Coordinates = new GeoCoordinate
            {
                Longitude = x.Coordinates[0],
                Latitude = x.Coordinates[1]
            },
            GeoHash = x.GeoHash
        });

        return locs;
    }
}
