using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Api.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL.Face
{
    public class FaceResolvers
    {
        public async Task<Person> GetPersonAsync(
            MediaFace face,
            PersonByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            if (face.PersonId.HasValue)
            {
                return await dataLoader.LoadAsync(face.PersonId.Value, cancellationToken);
            }

            return null;
        }
    }
}
