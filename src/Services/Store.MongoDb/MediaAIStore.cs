using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace MagicMedia.Store.MongoDb
{
    public class MediaAIStore : IMediaAIStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public MediaAIStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<MediaAI> GetByMediaIdAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.MediaAI.AsQueryable()
                .Where(x => x.MediaId == mediaId)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
