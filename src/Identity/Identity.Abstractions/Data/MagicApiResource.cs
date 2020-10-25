using IdentityServer4.Models;

namespace MagicMedia.Identity.Data
{
    public class MagicApiResource : ApiResource
    {
        public string Id => Name;
    }
}
