using System;

namespace MagicMedia;

public record CreateUserFromPersonRequest(Guid PersonId, string Email);
