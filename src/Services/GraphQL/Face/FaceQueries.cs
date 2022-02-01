using MagicMedia.Face;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face;

[ExtendObjectType(RootTypes.Query)]
public class FaceQueries
{
    public Task<SearchResult<MediaFace>> SearchFacesAsync(
        [Service] IFaceService faceService,
        SearchFacesRequest request,
        CancellationToken cancellationToken)
            => faceService.SearchAsync(request, cancellationToken);
}
