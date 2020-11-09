using System;
using System.Collections.Generic;

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
}
