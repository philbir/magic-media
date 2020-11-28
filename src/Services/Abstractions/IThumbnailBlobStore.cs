using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IThumbnailBlobStore
    {
        public Task StoreAsync(
            ThumbnailData data,
            CancellationToken cancellationToken);

        public Task StoreAsync(
            IEnumerable<ThumbnailData> datas,
            CancellationToken cancellationToken);

        public Task<byte[]> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }

    public record ThumbnailData(Guid Id, byte[] Data);
}
