using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<IEnumerable<ThumbnailData>> GetManyAsync(
            IEnumerable<Guid> id,
            CancellationToken cancellationToken);
    }

    public interface IMediaBlobStore
    {
        public Task StoreAsync(MediaBlobData data, CancellationToken cancellationToken);

        public Task<MediaBlobData> GetAsync(MediaBlobData request, CancellationToken cancellationToken);
    }

    public record ThumbnailData(Guid Id, byte[] Data);


    public record MediaBlobData
    {
        public string Filename { get; init; }

        public string Directory { get; init; } = "/";

        public byte[] Data { get; init; }

        public MediaBlobType Type { get; set; }
    }
}
