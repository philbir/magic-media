using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class ClientThumbprintStore : IClientThumbprintStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public ClientThumbprintStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<ClientThumbprint> TryGetByIdAsync(string id, CancellationToken cancellationToken)
        {
            ClientThumbprint? thumb = await _mediaStoreContext.ClientThumbprints.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return thumb;
        }

        public async Task AddAsync(
            ClientThumbprint clientThumbprint,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.ClientThumbprints.InsertOneAsync(
                clientThumbprint,
                DefaultMongoOptions.InsertOne,
                cancellationToken);
        }


        public async Task<IEnumerable<ClientThumbprint>> GetManyAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.ClientThumbprints.AsQueryable()
                .Where(x => ids.ToList().Contains(x.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
