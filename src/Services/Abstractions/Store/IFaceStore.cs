using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;

namespace MagicMedia.Store
{
    public interface IFaceStore
    {
        Task<IEnumerable<PersonEncodingData>> GetPersonEncodingsAsync(
            CancellationToken cancellationToken);

        Task<IEnumerable<MediaFace>> SearchAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken);
    }
}
