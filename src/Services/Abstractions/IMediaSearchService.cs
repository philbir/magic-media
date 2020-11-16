using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IMediaSearchService
    {
        Task<SearchResult<Media>> SearchAsync(SearchMediaRequest request, CancellationToken cancellationToken);
    }
}