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
using SixLabors.ImageSharp.ColorSpaces;

namespace MagicMedia.Store.MongoDb
{
    public class MongoMediaStore : IMediaStore
    {
        private readonly MediaStoreContext _mediaStoreContext;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public MongoMediaStore(
            MediaStoreContext mediaStoreContext,
            IThumbnailBlobStore thumbnailBlobStore,
            IFaceStore faceStore,
            ICameraStore cameraStore,
            IPersonStore personStore)
        {
            _mediaStoreContext = mediaStoreContext;
            _thumbnailBlobStore = thumbnailBlobStore;
            Faces = faceStore;
            Cameras = cameraStore;
            Persons = personStore;
        }

        public IFaceStore Faces { get; }

        public ICameraStore Cameras { get; }

        public IPersonStore Persons { get; }

        public async Task<SearchResult<Media>> SearchAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Media> filter = Builders<Media>.Filter.Empty;
          
            if (!string.IsNullOrEmpty(request.Folder))
            {
                filter &= Builders<Media>.Filter.Regex(
                    x => x.Folder,
                    new BsonRegularExpression("^" + Regex.Escape(request.Folder), "i"));
            }

            if (request.Persons is { } persons && persons.Any())
            {
                IEnumerable<Guid> mediaIds = await GetMediaIdsByPersons(
                    persons,
                    cancellationToken);

                if (mediaIds.Any())
                {
                    filter &= Builders<Media>.Filter.In(x => x.Id, mediaIds);
                }
            }

            if (request.Cities is { } cities && cities.Any())
            {
                filter &= Builders<Media>.Filter.In(
                    x => x.GeoLocation.Address.City,
                    cities);
            }

            if (request.Countries is { } countries && countries.Any())
            {
                filter &= Builders<Media>.Filter.In(
                    x => x.GeoLocation.Address.CountryCode,
                    countries);
            }

            IFindFluent<Media, Media>? cursor = _mediaStoreContext.Medias.Find(filter);
            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<Media> medias = await cursor
                .SortByDescending(x => x.Source.ImportedAt)
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<Media>(medias, (int)totalCount);
        }

        private async Task<IEnumerable<Guid>> GetMediaIdsByPersons(
            IEnumerable<Guid> persons,
            CancellationToken cancellationToken)
        {
            List<Guid> ids = await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.PersonId.HasValue)
                .Where(x => persons.ToList().Contains(x.PersonId.Value))
                .Select(x => x.MediaId)
                .ToListAsync(cancellationToken);

            return ids;
        }

        public async Task<Media> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStoreContext.Medias.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return media;
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
                MediaThumbnail thumb = media.Thumbnails.Where(x => x.Size == size).FirstOrDefault();
                if (thumb != null)
                {
                    thumb.Data = await _thumbnailBlobStore.GetAsync(thumb.Id, cancellationToken);
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
                    await _thumbnailBlobStore.StoreAsync(
                        new ThumbnailData(face.Thumbnail.Id, face.Thumbnail.Data),
                        cancellationToken);

                    face.Thumbnail.Data = null;
                }

                if (faces.Any())
                {
                    await _mediaStoreContext.Faces.InsertManyAsync(
                        faces,
                        options: null,
                        cancellationToken);
                }
            }

            await _mediaStoreContext.Medias.UpdateOneAsync(
                x => x.Id == mediaId,
                Builders<Media>.Update.Set(f => f.FaceCount, faces.Count()),
                options: null,
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

        public async Task InsertMediaAsync(
            Media media,
            IEnumerable<MediaFace> faces,
            CancellationToken cancellationToken)
        {
            foreach (MediaThumbnail thumb in media.Thumbnails)
            {
                await _thumbnailBlobStore.StoreAsync(
                    new ThumbnailData(thumb.Id, thumb.Data),
                    cancellationToken);

                thumb.Data = null;
            }

            await _mediaStoreContext.Medias.InsertOneAsync(
                media,
                options: null,
                cancellationToken);

            if (faces != null)
            {
                foreach (MediaFace face in faces)
                {
                    await _thumbnailBlobStore.StoreAsync(
                        new ThumbnailData(face.Thumbnail.Id, face.Thumbnail.Data),
                        cancellationToken);

                    face.Thumbnail.Data = null;
                }
            }

            if (faces.Any())
            {
                await _mediaStoreContext.Faces.InsertManyAsync(
                    faces,
                    options: null,
                    cancellationToken);
            }
        }

        public async Task<IEnumerable<string>> GetAllFoldersAsync(CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Medias.AsQueryable()
                .Select(x => x.Folder)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetGroupedCitiesAsync(
                CancellationToken cancellationToken)
        {
            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.Media,
                "Media_GroupByCity",
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

        public async Task<IEnumerable<SearchFacetItem>> GetGroupedCountriesAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.Media,
                "Media_GroupByCountry",
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
    }
}
