using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
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

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, id.ToString("N"));

            IAsyncCursor<GridFSFileInfo> cursor = await _gridFSBucket.FindAsync(filter, options: null, cancellationToken);
            GridFSFileInfo? file = cursor.FirstOrDefault(cancellationToken);

            if (file != null)
            {
                await _gridFSBucket.DeleteAsync(file.Id, cancellationToken);

                return true;
            }

            return false;
        }


        public Task StoreAsync(IEnumerable<ThumbnailData> datas, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
