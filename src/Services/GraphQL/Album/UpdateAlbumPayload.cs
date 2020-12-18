using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UpdateAlbumPayload : Payload
    {
        public Album? Album { get; }

        public UpdateAlbumPayload(Album album)
        {
            Album = album;
        }

        public UpdateAlbumPayload(IReadOnlyList<UserError>? errors)
            : base(errors)
        {
        }
    }
}
