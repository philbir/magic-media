using System;

namespace MagicMedia.Messaging
{
    public record PersonUpdatedMessage(Guid Id, string Action);
 
    public record FavoriteMediaToggledMessage(Guid Id, bool IsFavorite);
}
