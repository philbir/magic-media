using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL.Face
{
    [ExtendObjectType(Name = "Query")]
    public class FaceQueries
    {
        private readonly IMediaStore _mediaStore;

        public FaceQueries(IMediaStore mediaStore)
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
