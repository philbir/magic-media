using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{

    public class FaceUpdatedMessage
    {
        public Guid Id { get; set; }

        public Guid PersonId { get; set; }

        public string Action { get; set; }

        public FaceUpdatedMessage(Guid id, string action)
        {
            Id = id;
            Action = action;
        }

        public FaceUpdatedMessage(Guid id, Guid personId, string action)
        {
            Id = id;
            PersonId = personId;
            Action = action;
        }
    }
}
