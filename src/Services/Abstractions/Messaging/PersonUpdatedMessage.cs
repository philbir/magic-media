using System;

namespace MagicMedia.Messaging
{
    public record PersonUpdatedMessage(Guid Id, string Action);

    public record PersonDeletedMessage(Guid Id, ClientInfo ClientInfo);

    public record FavoriteMediaToggledMessage(Guid Id, bool IsFavorite);
}
