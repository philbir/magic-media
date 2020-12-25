using System;
using System.Collections.Generic;

namespace MagicMedia.Messaging
{
    public record ItemsAddedToAlbumMessage(Guid Id);

    public record FoldersRemovedFromAlbum(Guid Id, IEnumerable<string> Folders);

    public class AlbumDeletedMessage : UserContextMessage
    {
        public AlbumDeletedMessage(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class DeleteAlbumMessage : UserContextMessage
    {
        public Guid Id { get; set; }
    }

    public class UserContextMessage : IUserContextMessage
    {
        public ClientInfo? ClientInfo { get; set; }

        public Guid UserId { get; set; }
    }

}
