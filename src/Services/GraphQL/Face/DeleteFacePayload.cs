using System;
using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    public class DeleteFacePayload : Payload
    {
        public Guid Id { get; }

        public DeleteFacePayload(Guid id)
        {
            Id = id;
        }

        public DeleteFacePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }

    public class PredictPersonPayload : Payload
    {
        public bool HasMatch { get; }
        public MediaFace? Face { get; }

        public PredictPersonPayload(bool hasMatch, MediaFace face)
        {
            HasMatch = hasMatch;
            Face = face;
        }

        public PredictPersonPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
