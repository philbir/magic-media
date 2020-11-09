using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Operations;

namespace MagicMedia.Store.MongoDb
{
    public class OperationStore : IOperationStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public OperationStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }


        public async Task AddAsync(MediaOperation operation, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Operations.InsertOneAsync(
                operation,
                options: null,
                cancellationToken);
        }
    }
}
