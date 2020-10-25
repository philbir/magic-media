using IdentityServer4.Models;

namespace MagicMedia.Identity.Data
{
    public class MagicApiScope : ApiScope
    {
        public string Id => Name;
    }
}
