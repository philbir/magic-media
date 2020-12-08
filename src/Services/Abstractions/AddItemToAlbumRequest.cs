using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class AddItemToAlbumRequest
    {
        public Guid? AlbumId { get; set; }

        public string? NewAlbumTitle { get; set; }

        public IEnumerable<Guid>? MediaIds { get; set; }

        public IEnumerable<string>? Folders { get; set; }
    }

    public record RemoveFoldersFromAlbumRequest(Guid AlbumId, IEnumerable<string> Folders);
}
