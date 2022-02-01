using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face;

public class FaceType : ObjectType<MediaFace>
{
    protected override void Configure(IObjectTypeDescriptor<MediaFace> descriptor)
    {
        descriptor
            .Field("person")
            .ResolveWith<Resolvers>(x => x.GetPersonAsync(default!, default!, default!));

        descriptor
            .Field("thumbnail")
            .ResolveWith<Resolvers>(x => x.GetThumbnail(default!));

        descriptor
            .Field("media")
            .ResolveWith<Resolvers>(x => x.GetMediaAsync(default!, default!, default!));
    }

    class Resolvers
    {
        public async Task<Person?> GetPersonAsync(
            [Parent] MediaFace face,
            [DataLoader] PersonByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            if (face.PersonId.HasValue)
            {
                return await dataLoader.LoadAsync(face.PersonId.Value, cancellationToken);
            }

            return null;
        }

        public async Task<Media> GetMediaAsync(
            [Parent] MediaFace face,
            [DataLoader] MediaByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(face.MediaId, cancellationToken);
        }

        public MediaThumbnail? GetThumbnail(
            [Parent] MediaFace face)
        {
            MediaThumbnail? thumb = face.Thumbnail;
            if (thumb != null)
            {
                thumb.Owner = new ThumbnailOwner
                {
                    Id = face.Id,
                    Type = ThumbnailOwnerType.Face
                };
            }

            return thumb;
        }
    }
}
