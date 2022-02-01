using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face;

public class UpdateFacesPayload : Payload
{
    public IEnumerable<MediaFace>? Faces { get; }

    public UpdateFacesPayload(IEnumerable<MediaFace> faces)
    {
        Faces = faces;
    }

    public UpdateFacesPayload(IReadOnlyList<UserError>? errors = null)
        : base(errors)
    {
    }
}
