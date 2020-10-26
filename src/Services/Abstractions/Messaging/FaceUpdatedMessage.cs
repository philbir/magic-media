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

        public string Action { get; set; }
    }
}
