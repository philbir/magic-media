using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class User
    {
        public Guid Id { get; set; }

        public string  Name { get; set; }

        public string Email { get; set; }

        public Guid? PersonId { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
