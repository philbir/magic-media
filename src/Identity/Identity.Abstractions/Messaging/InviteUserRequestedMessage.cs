using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Identity.Data;

namespace MagicMedia.Messaging
{
    public record InviteUserRequestedMessage(Guid UserId, string Name, string Email);

    public record InviteUserCreatedMessage(Guid UserId, string Name, string Email, string Code);
}
