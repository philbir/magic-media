using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
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

        public async Task<Media> GetMediaAsync(
            MediaFace face,
            MediaByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(face.MediaId, cancellationToken);
        }
    }
}
