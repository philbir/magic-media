using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{
    public class PersonUpdatedMessage
    {
        public Guid Id { get; set; }

        public string Action { get; set; }

        public PersonUpdatedMessage(Guid id, string action)
        {
            Id = id;
            Action = action;
        }
    }
}
