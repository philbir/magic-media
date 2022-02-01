using System;

namespace MagicMedia.Messaging;

public record InviteUserRequestedMessage(Guid UserId, string Name, string Email);
