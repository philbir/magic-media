using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace Api.Host.GraphQL
{
    public class Query
    {
        private readonly IMediaStore _mediaStore;

        public Query(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public async Task<IEnumerable<MediaFace>> SearchFacesAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.Faces.SearchAsync(request, cancellationToken);
        }
    }
}
