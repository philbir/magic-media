using System;
using System.Collections.Generic;

namespace MagicMedia.Messaging
{
    public record ItemsAddedToAlbumMessage(Guid Id);

    public record FoldersRemovedFromAlbum(Guid Id, IEnumerable<string> Folders);
}
