using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class FaceStore : IFaceStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public FaceStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<SearchResult<MediaFace>> SearchAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<MediaFace> filter = Builders<MediaFace>.Filter.Empty;

            if (request.States is { } states && states.Any())
            {
                filter = filter & Builders<MediaFace>.Filter.In(x => x.State, states);
            }
            if (request.RecognitionTypes is { } types && types.Any())
            {
                filter = filter & Builders<MediaFace>.Filter.In(x => x.RecognitionType, types);
            }
            if (request.Persons is { } persons && persons.Any())
            {
                filter = filter & Builders<MediaFace>.Filter.In(nameof(MediaFace.PersonId), persons);
            }

            IFindFluent<MediaFace, MediaFace> cursor = _mediaStoreContext.Faces.Find(filter);

            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<MediaFace> faces = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<MediaFace>(faces, (int)totalCount);
        }

        public async Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.MediaId == mediaId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MediaFace>> GetFacesByPersonAsync(
            Guid personId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PersonEncodingData>> GetPersonEncodingsAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.State == FaceState.Validated && x.PersonId != null)
                .Select(x => new PersonEncodingData
                {
                    PersonId = x.PersonId.Value,
                    Encoding = x.Encoding
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            MediaFace face = await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.Id == id)
                .FirstAsync(cancellationToken);

            return face;
        }

        public async Task UpdateAsync(MediaFace face, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Faces.ReplaceOneAsync(
                x => x.Id == face.Id,
                face,
                options: new ReplaceOptions(),
                cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Faces.DeleteOneAsync(
                x => x.Id == id,
                options: new DeleteOptions(),
                cancellationToken);
        }

        public async Task BulkUpdateAgesAsync(
            IEnumerable<UpdateAgeRequest> updates,
            CancellationToken cancellationToken)
        {
            List<UpdateOneModel<MediaFace>> bulkUpdates = new ();
            foreach (UpdateAgeRequest upd in updates)
            {
                FilterDefinition<MediaFace>? filter = Builders<MediaFace>.Filter
                    .Eq(x => x.Id, upd.Id);

                UpdateDefinition<MediaFace>? update = Builders<MediaFace>.Update
                    .Set(x => x.Age, upd.Age);

                bulkUpdates.Add(new UpdateOneModel<MediaFace>(filter, update));
            }

            await _mediaStoreContext.Faces.BulkWriteAsync(
                bulkUpdates,
                options: null,
                cancellationToken);
        }
    }
}
