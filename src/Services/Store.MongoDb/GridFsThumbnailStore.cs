using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.GridFS;

namespace MagicMedia.Store.MongoDb
{
    public class GridFsThumbnailStore : IThumbnailBlobStore
    {
        private readonly IGridFSBucket _gridFSBucket;

        public GridFsThumbnailStore(IGridFSBucket gridFSBucket)
        {
            _gridFSBucket = gridFSBucket;
        }

        public Task<byte[]> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return _gridFSBucket.DownloadAsBytesByNameAsync(
                id.ToString("N"), null, cancellationToken);
        }

        public async Task StoreAsync(ThumbnailData data, CancellationToken cancellationToken)
        {
            await _gridFSBucket.UploadFromBytesAsync(
                data.Id.ToString("N"),
                data.Data,
                options: null,
                cancellationToken);
        }

        public Task StoreAsync(IEnumerable<ThumbnailData> datas, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
