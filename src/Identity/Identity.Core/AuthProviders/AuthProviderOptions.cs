using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Identity.AuthProviders
{
    public class AuthProviderOptions
    {
        public string Name { get; set; }

        public string ClientId { get; set; }

        public string Secret { get; set; }
    }
}
