using System;
using System.Collections.Generic;

namespace MagicMedia.Identity
{
    public class InviteViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsValid { get; internal set; }
        public List<ExternalProvider> Providers { get; internal set; }
    }
}
