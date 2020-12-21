using System;

namespace MagicMedia.Messaging
{
    public record InviteUserCreatedMessage(Guid UserId, string Name, string Email, string Code);

    public record UserAccountCreatedMessage(Guid UserId);
}
