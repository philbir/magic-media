using System;
using System.Collections.Generic;

namespace MagicMedia.GraphQL
{
    public class DeleteGroupPayload : Payload
    {
        public DeleteGroupPayload(Guid id)
        {
            Id = id;
        }

        public DeleteGroupPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }

        public Guid? Id { get; }
    }

}
