using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public interface IMediaAIStore
    {
        Task<MediaAI> GetByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken);
    }
}
