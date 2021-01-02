using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public class AuditEvent
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid? UserId { get; set; }

        public AuditResource? Resource { get; set; }

        public ClientInfo Client { get; set; }

        public string Hostname { get; set; }
        public bool Success { get; set; }
        public string? Action { get; set; }
        public string? GrantFrom { get; set; }
    }

    public class AuditResource
    {
        public ProtectedResourceType Type { get; set; }

        public string? Id { get; set; }
        public string? Raw { get; set; }
    }
}
