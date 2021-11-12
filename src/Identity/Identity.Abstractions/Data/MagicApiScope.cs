using Duende.IdentityServer.Models;

namespace MagicMedia.Identity.Data
{
    public class MagicApiScope : ApiScope
    {
        public string Id => Name;
    }
}
