using System;

namespace MagicMedia.Messaging
{
    public record PersonUpdatedMessage(Guid Id, string Action);

    public class PersonDeletedMessage : UserContextMessage
    {
        public PersonDeletedMessage(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public record FavoriteMediaToggledMessage(Guid Id, bool IsFavorite);
}
