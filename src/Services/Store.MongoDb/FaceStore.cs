using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
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

        public async Task<IEnumerable<MediaFace>> SearchAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<MediaFace> filter = Builders<MediaFace>.Filter.Empty;

            if (request.States is { } states)
            {
                filter = filter & Builders<MediaFace>.Filter.In(x => x.State, states);
            }

            List<MediaFace> faces = await _mediaStoreContext.Faces.Find(filter)
                .ToListAsync(cancellationToken);

            return faces;
        }

        public async Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Faces.AsQueryable()
                .Where(x => x.MediaId == mediaId)
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
    }
}
