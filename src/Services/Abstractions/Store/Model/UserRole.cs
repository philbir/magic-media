using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class UserRole
    {
        public string Id { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
