using System;
using System.Collections.Generic;

namespace MagicMedia.GraphQL
{
    public class ToggleMediaFavoritePayload : Payload
    {
        public ToggleMediaFavoritePayload(Guid id, bool isFavorite)
        {
            Id = id;
            IsFavorite = isFavorite;
        }

        public Guid Id { get; set; }

        public bool IsFavorite { get; set; }

        public ToggleMediaFavoritePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
