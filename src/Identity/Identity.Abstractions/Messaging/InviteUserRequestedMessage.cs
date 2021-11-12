using System;

namespace MagicMedia.Messaging
{
    public record InviteUserRequestedMessage(Guid UserId, string Name, string Email);

    public record InviteUserCreatedMessage(Guid UserId, string Name, string Email, string Code);

    public record UserAccountCreatedMessage(Guid UserId);
}
