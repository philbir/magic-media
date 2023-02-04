using System;

namespace MagicMedia.Messaging;

public interface IUserContextMessage
{
    ClientInfo? ClientInfo { get; set; }
    Guid UserId { get; set; }
}
