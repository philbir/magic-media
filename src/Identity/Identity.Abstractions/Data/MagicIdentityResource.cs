using IdentityServer4.Models;

namespace MagicMedia.Identity.Data
{
    public class MagicIdentityResource : IdentityResource
    {
        public string Id => Name;
    }
}
