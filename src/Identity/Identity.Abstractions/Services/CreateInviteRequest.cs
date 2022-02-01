using System;

namespace MagicMedia.Identity.Services;

public record CreateInviteRequest(Guid UserId, string Name, string Email);
