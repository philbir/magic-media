using System;
using System.Collections.Generic;

namespace MagicMedia.GraphQL
{
    public class DeleteAlbumPayload : Payload
    {
        public DeleteAlbumPayload(Guid id)
        {
            Id = id;
        }

        public DeleteAlbumPayload(IReadOnlyList<UserError>? errors)
            : base(errors)
        {
        }

        public Guid? Id { get; }
    }
}
