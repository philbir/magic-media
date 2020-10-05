using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    internal class FaceStore : IFaceStore
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
    }
}
