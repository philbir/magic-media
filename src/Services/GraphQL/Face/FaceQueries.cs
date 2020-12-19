using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Face;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    [ExtendObjectType(Name = "Query")]
    public class FaceQueries
    {
        private readonly IFaceService _faceService;

        public FaceQueries(IFaceService faceService)
        {
            _faceService = faceService;
        }

        public async Task<SearchResult<MediaFace>> SearchFacesAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken)
        {
            return await _faceService.SearchAsync(request, cancellationToken);
        }
    }
}
