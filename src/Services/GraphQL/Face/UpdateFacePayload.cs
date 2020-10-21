using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    public class UpdateFacePayload : Payload
    {
        public MediaFace Face { get; }

        public UpdateFacePayload(MediaFace face)
        {
            Face = face;
        }

        public UpdateFacePayload(IReadOnlyList<UserError> errors = null)
            : base(errors)
        {
        }
    }
}
