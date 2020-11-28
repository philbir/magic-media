using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{
    public record FaceUpdatedMessage(Guid Id, string Action)
    {
        public Guid PersonId { get; init; }
    }
}
